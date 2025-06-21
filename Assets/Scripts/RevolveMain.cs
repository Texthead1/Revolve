using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using PortalToUnity;
using TMPro;
using UnityEngine;

namespace Revolve
{
    public class RevolveMain : MonoBehaviour
    {
        [SerializeField] private bool throwAlertsOnIncompatibleFigures;
        [SerializeField] private TextMeshProUGUI villainNameText;
        [SerializeField] private TextMeshProUGUI villainSuperText;
        [SerializeField] private TextMeshProUGUI figureNameText;
        [SerializeField] private GameObject magicMoment;
        [SerializeField] private GameObject readWriteAlert;
        [SerializeField] private GameObject tooManyPortalsAlert;
        [SerializeField] private GameObject portalDisconnectAlert;
        [SerializeField] private GameObject noSaltAlert;

        private PortalOfPower primaryPortal;
        private PortalFigure modifyingTrap;
        private List<PortalFigure> backgroundTraps = new List<PortalFigure>();

        private void OnEnable()
        {
            if (!Cryptography.SaltIsReady()) return;
            PortalOfPower.OnAdded += PortalAdded;
            PortalOfPower.OnRemoved += PortalRemoved;
        }

        private void OnDisable()
        {
            PortalOfPower.OnAdded -= PortalAdded;
            PortalOfPower.OnRemoved -= PortalRemoved;
        }

        private void Awake()
        {
            Cryptography.CheckForSalt();

            magicMoment.SetActive(false);
            readWriteAlert.SetActive(false);
            tooManyPortalsAlert.SetActive(false);
            noSaltAlert.SetActive(false);

            if (!Cryptography.SaltIsReady())
            {
                noSaltAlert.SetActive(true);
                return;
            }
            portalDisconnectAlert.SetActive(true);
        }

        private string AppendElementSymbols(string name, Element element)
        {
            if (element == Element.None)
                return name;

            if (element == Element.Kaos)
                return $"! {name} !";

            int symbol = ((int)element) - 1;
            return $"{symbol} {name} {symbol}";
        }

        private string EvolvedState(bool value) => value ? "Evolved" : "Non-Evolved";

        private void PortalAdded(PortalOfPower portal)
        {
            // already plugged in portals
            if (PortalOfPower.Instances.Count > 1)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => tooManyPortalsAlert.SetActive(true));

                // subscribe to background portal alerts
                Subscribe(portal);
                return;
            }

            // single portal, proceed to magic moment
            primaryPortal = portal;
            Subscribe(portal);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                magicMoment.SetActive(true);
                portalDisconnectAlert.SetActive(false);
            });

            primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);
        }

        private void PortalRemoved(PortalOfPower portal)
        {
            // unsubscrible from portal events, if primary or background portal
            Unsubscribe(portal);

            if (portal == primaryPortal)
            {
                backgroundTraps = new List<PortalFigure>();
                primaryPortal = null;
            }

            if (PortalOfPower.Instances.Count == 1)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => tooManyPortalsAlert.SetActive(false));
                PortalOfPower newPortal = PortalOfPower.Instances.FirstOrDefault();
                PortalAdded(newPortal);
                return;
            }

            if (PortalOfPower.Instances.Count == 0)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => portalDisconnectAlert.SetActive(true));
                return;
            }
        }

        private void Subscribe(PortalOfPower portal)
        {
            portal.OnFigureAdded += OnFigureAdded;
            portal.OnFigureRemoved += OnFigureRemoved;
        }

        private void Unsubscribe(PortalOfPower portal)
        {
            portal.OnFigureAdded -= OnFigureAdded;
            portal.OnFigureRemoved -= OnFigureRemoved;
        }

        private unsafe (ushort, VariantID) GetCharacterAndVariantIDs(PortalFigure figure)
        {
            ushort characterID = figure.TagHeader->toyType;
            VariantID variantID = new VariantID(figure.TagHeader->subType);
            return (characterID, variantID);
        }

        private async void OnFigureAdded(PortalFigure figure)
        {
            // respond to figure if added to primary portal
            if (figure.Parent == primaryPortal)
            {
                if (figure.Parent.currentlyQueryingFigure == null && PortalOfPower.Instances.Count == 1)
                    await RespondToFigure(figure);
                else
                    figure.Parent.FiguresInQueue.Add(figure);

                return;
            }

            // simply queue figure if on a background portal for automatic discovery on primary portal unplug
            figure.Parent.FiguresInQueue.Add(figure);
        }

        private async Task RespondToFigure(PortalFigure figure)
        {
            primaryPortal.currentlyQueryingFigure = figure;
            bool isTrap = false;

            try
            {
                await figure.FetchTagHeader();
                (ushort characterID, VariantID variantID) = GetCharacterAndVariantIDs(figure);

                if (!SkylanderDatabase.GetSkylander(characterID, out Skylander skylander))
                {
                    if (throwAlertsOnIncompatibleFigures)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            GameObject obj = Instantiate(Resources.Load<GameObject>("unknownFigure"), transform.parent);
                            obj.name = $"{figure.Parent.SessionID}{figure.Index}";
                            obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                        });
                    }
                    return;
                }

                if (skylander.Type != SkyType.Trap)
                {
                    if (throwAlertsOnIncompatibleFigures)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            GameObject obj = Instantiate(Resources.Load<GameObject>("incompatibleFigure"), transform.parent);
                            obj.name = $"{figure.Parent.SessionID}{figure.Index}";
                            obj.GetComponentInChildren<FigureDescription>().CreateAlert(figure.GetExposableSpyroTagName());
                            obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                        });
                    }
                    return;
                }

                if (modifyingTrap != null)
                {
                    // we assume another trap is on the portal.
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GameObject obj = Instantiate(Resources.Load<GameObject>("tooManyTraps"), transform.parent);
                        obj.name = $"{figure.Parent.SessionID}{figure.Index}";
                        obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                    });
                    backgroundTraps.Add(figure);
                    return;
                }

                isTrap = true;
                await RespondToTrap(figure, skylander);
            }
            catch (FigureRemovedException)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => readWriteAlert.SetActive(false));
                primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);
            }
            catch (FigureErrorException)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>(isTrap ? "errorFigure" : "unknownFigure"), transform.parent);
                    obj.name = $"{figure.Parent.SessionID}{figure.Index}";
                    obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                    readWriteAlert.SetActive(false);
                });
                primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);
            }
            catch (PortalIOException)
            {
                if (throwAlertsOnIncompatibleFigures)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GameObject obj = Instantiate(Resources.Load<GameObject>(isTrap ? "errorFigure" : "unknownFigure"), transform.parent);
                        obj.name = $"{figure.Parent.SessionID}{figure.Index}";
                        obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                        readWriteAlert.SetActive(false);
                    });
                }
                primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);
            }
            finally
            {
                primaryPortal.currentlyQueryingFigure = null;
                if (primaryPortal.FiguresInQueue.Count > 0 && PortalOfPower.Instances.Count == 1)
                {
                    PortalFigure queuedFigure = primaryPortal.FiguresInQueue.FirstOrDefault();
                    primaryPortal.FiguresInQueue.Remove(queuedFigure);
                    await RespondToFigure(queuedFigure);
                }
            }
        }

        private async Task RespondToTrap(PortalFigure figure, Skylander trapInfo)
        {
            modifyingTrap = figure;

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                magicMoment.SetActive(false);
                readWriteAlert.transform.Find("ReadWriteLabel").GetComponent<TextMeshProUGUI>().text = "Reading. Please wait...";
                readWriteAlert.SetActive(true);
            });

            figure.TagBuffer = new FigType_Trap(figure);
            FigType_Trap trap = (FigType_Trap)figure.TagBuffer;

            PortalInfo portalInfo = figure.Parent.GetPortalInfo();

            int colorIndex = (int)LEDType.FullColor;
            if (portalInfo != null)
            {
                colorIndex = portalInfo.LEDType == LEDType.Enhanced ? (int)LEDType.FullColor : (int)portalInfo.LEDType;

                if (portalInfo.LEDType != LEDType.None)
                    primaryPortal.COMMAND_SetLEDColor(Elements.Colors[trapInfo.Element][colorIndex]);
            }
            else
                primaryPortal.COMMAND_SetLEDColor(Elements.Colors[trapInfo.Element][colorIndex]);

            await trap.FetchMagicMoment();
            await trap.FetchRemainingData();

            unsafe
            {
                if (!VillainDatabase.GetVillain(trap.SpyroTag->magicMoment.primaryVillain.villainID, out Villain villain))
                {
                    ShowNoVillainAlert(figure);

                    primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);
                    return;
                }

                // fallback name
                string name = trapInfo.Name;

                unsafe
                {
                    VariantID variantID = new VariantID(figure.TagHeader->subType);

                    // this version of Portal-To-Unity doesn't currently have the built-in variant lookup algo
                    // so a simplified version is implemented here
                    foreach (SkylanderVariant trapVariant in trapInfo.Variants)
                    {
                        if (trapVariant.VariantID.DecoID == variantID.DecoID && trapVariant.VariantID.IsAltDeco == variantID.IsAltDeco)
                        {
                            name = trapVariant.Name;
                            break;
                        }
                    }
                }

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    figureNameText.text = name;
                    readWriteAlert.SetActive(false);
                    villainNameText.text = AppendElementSymbols(villain.Name, trapInfo.Element);
                    villainSuperText.text = (trap.SpyroTag->magicMoment.primaryVillain.isEvolved == 1) ? "Evolved" : "Non-Evolved";

                    if (trap.SpyroTag->magicMoment.variantVillainID == trap.SpyroTag->magicMoment.primaryVillain.villainID && villain.Variant != null)
                        villainSuperText.text = string.Join(" ", villainSuperText.text, villain.Variant.Name);
                });
            }

            // rest of logic, read trap, show reading alert, go to modify menu, you know the drill
            primaryPortal.COMMAND_SetLEDColor(0xFF, 0xFF, 0xFF);
        }

        public async void WriteEvolveSelection(bool evolve)
        {
            try
            {
                FigType_Trap trap = (FigType_Trap)modifyingTrap.TagBuffer;

                unsafe
                {
                    trap.SpyroTag->magicMoment.primaryVillain.isEvolved = (byte)(evolve ? 1 : 0);
                }

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    readWriteAlert.transform.Find("ReadWriteLabel").GetComponent<TextMeshProUGUI>().text = "Writing. Please wait...";
                    readWriteAlert.SetActive(true);
                });

                primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);

                await trap.SetMagicMoment();
                await trap.SetRemainingData();

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("writeComplete"), transform.parent);
                    obj.name = $"{modifyingTrap.Parent.SessionID}{modifyingTrap.Index}";
                    obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                    readWriteAlert.SetActive(false);
                });

                primaryPortal.COMMAND_SetLEDColor(0x00, 0xFF, 0x10);
            }
            catch (FigureRemovedException)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => readWriteAlert.SetActive(false));
            }
            catch (FigureErrorException)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("errorFigure"), transform.parent);
                    obj.name = $"{modifyingTrap.Parent.SessionID}{modifyingTrap.Index}";
                    obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                    readWriteAlert.SetActive(false);
                });
            }
            catch (PortalIOException)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("errorFigure"), transform.parent);
                    obj.name = $"{modifyingTrap.Parent.SessionID}{modifyingTrap.Index}";
                    obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                    readWriteAlert.SetActive(false);
                });
            }
        }

        private void ShowNoVillainAlert(PortalFigure figure)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>("noVillain"), transform.parent);
                obj.name = $"{figure.Parent.SessionID}{figure.Index}";
                obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                readWriteAlert.SetActive(false);
            });
        }

        private async void OnFigureRemoved(PortalFigure figure, FigureDepartInfo info)
        {
            if (backgroundTraps.Contains(figure))
            {
                backgroundTraps.Remove(figure);

                if (backgroundTraps.Count > 1)
                {
                    PortalFigure targetTrap = backgroundTraps.FirstOrDefault();
                    RemoveFigureAlert(targetTrap);
                    return;
                }
            }

            RemoveFigureAlert(figure);

            if (figure == modifyingTrap)
            {
                modifyingTrap = null;

                if (backgroundTraps.Count > 0)
                {
                    PortalFigure newTrap = backgroundTraps.FirstOrDefault();
                    RemoveFigureAlert(newTrap);

                    backgroundTraps.Remove(newTrap);

                    (ushort characterID, VariantID variantID) = GetCharacterAndVariantIDs(newTrap);
                    SkylanderDatabase.GetSkylander(characterID, out Skylander trapInfo);

                    try
                    {
                        await RespondToTrap(newTrap, trapInfo);
                    }
                    catch (FigureRemovedException)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => readWriteAlert.SetActive(false));
                    }
                    catch (FigureErrorException)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            GameObject obj = Instantiate(Resources.Load<GameObject>("errorFigure"), transform.parent);
                            obj.name = $"{newTrap.Parent.SessionID}{newTrap.Index}";
                            obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                            readWriteAlert.SetActive(false);
                        });
                    }
                    catch (PortalIOException)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            GameObject obj = Instantiate(Resources.Load<GameObject>("errorFigure"), transform.parent);
                            obj.name = $"{newTrap.Parent.SessionID}{newTrap.Index}";
                            obj.transform.SetSiblingIndex(readWriteAlert.transform.GetSiblingIndex() + 1);
                            readWriteAlert.SetActive(false);
                        });
                    }
                    finally
                    {
                        primaryPortal.currentlyQueryingFigure = null;
                        if (primaryPortal.FiguresInQueue.Count > 0 && PortalOfPower.Instances.Count == 1)
                        {
                            PortalFigure queuedFigure = primaryPortal.FiguresInQueue.FirstOrDefault();
                            primaryPortal.FiguresInQueue.Remove(queuedFigure);
                            await RespondToFigure(queuedFigure);
                        }
                    }
                    return;
                }
                UnityMainThreadDispatcher.Instance().Enqueue(() => magicMoment.SetActive(true));
            }
        }

        private void RemoveFigureAlert(PortalFigure figure)
        {
            if (figure == modifyingTrap)
                primaryPortal.COMMAND_SetLEDColor(0x00, 0x00, 0x00);
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Transform target = transform.parent.Find($"{figure.Parent.SessionID}{figure.Index}");

                if (target != null)
                    Destroy(target.gameObject);
            });
        }
    }
}