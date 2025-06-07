using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using static PortalToUnity.Global;

#if UNITY_EDITOR
namespace PortalToUnity
{
    public class FigureDebugger : EditorWindow
    {
        private static List<PortalOfPower> activePortals = new List<PortalOfPower>();
        private static PortalOfPower selectedPortal;
        private static PortalFigure selectedFigure;

        private Vector2 portalScrollPos;
        private Vector2 figureScrollPos;
        private Vector2 infoScrollPos;
        private Dictionary<SpyroTag_TagHeader, bool> headerFoldoutStates = new Dictionary<SpyroTag_TagHeader, bool>();
        private float portalPanelWidth = 0.25f;
        private float figurePanelWidth = 0.25f;
        private static bool figureInFocus;

        [MenuItem("Portal-To-Unity/Figure Debugger", false, 0)]
        public static void ShowWindow()
        {
            FigureDebugger window = GetWindow<FigureDebugger>("Figure Debugger");
            window.titleContent = new GUIContent("Figure Debugger", Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Portal-To-Unity/Editor/Icons/d_placeholder" : "Portal-To-Unity/Editor/Icons/d_placeholder"));
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.fontSize = 14;

            if (!EditorApplication.isPlaying)
            {
                GUILayout.FlexibleSpace();
                boldStyle.wordWrap = true;
                selectedPortal = null;
                selectedFigure = null;
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width - 10));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Editor application must be playing to inspect active input handling", boldStyle);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                return;
            }

            if (activePortals.Count == 0)
            {
                boldStyle.wordWrap = true;
                selectedPortal = null;
                selectedFigure = null;
                portalScrollPos = EditorGUILayout.BeginScrollView(portalScrollPos);
                EditorGUILayout.LabelField("Please have at least one Portal of Power plugged connected to inspect", boldStyle);
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
                return;
            }

            portalScrollPos = EditorGUILayout.BeginScrollView(portalScrollPos, GUILayout.Width(position.width * portalPanelWidth));
            foreach (PortalOfPower portal in activePortals)
            {
                if (GUILayout.Button(portal.GetName()) && Event.current.button != 1)
                    SetSelection(portal);
            }
            EditorGUILayout.EndScrollView();

            if (selectedPortal == null)
            {
                boldStyle.wordWrap = true;
                figureScrollPos = EditorGUILayout.BeginScrollView(figureScrollPos, GUILayout.Width(position.width * (1 - portalPanelWidth)));
                EditorGUILayout.LabelField("Please select an active Portal of Power", boldStyle);
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
                return;
            }

            figureScrollPos = EditorGUILayout.BeginScrollView(figureScrollPos, GUILayout.Width(position.width * figurePanelWidth));
            List<PortalFigure> existingFigures = selectedPortal.Figures.Where(x => x.IsPresent()).ToList();

            if (existingFigures.Count() != 0)
            {
                foreach (PortalFigure figure in existingFigures)
                {
                    if (GUILayout.Button(figure.GetExposableSpyroTagName()) && Event.current.button != 1)
                        SetSelection(figure);
                }
            }
            else
            {
                boldStyle.wordWrap = true;
                EditorGUILayout.LabelField("No figures on Portal", boldStyle);
                boldStyle.wordWrap = false;
            }
            EditorGUILayout.EndScrollView();

            Font mono = Resources.Load<Font>("Portal-To-Unity/Editor/Fonts/Consolas");
            GUIStyle bufferFont = new GUIStyle(EditorStyles.label)
            {
                font = mono,
                fontSize = 13,
            };
            
            if (figureInFocus)
            {
                if (selectedFigure == null)
                {
                    EditorGUILayout.EndHorizontal();
                    return;
                }

                // handle figure info printing
                infoScrollPos = EditorGUILayout.BeginScrollView(infoScrollPos, GUILayout.Width(position.width * (1 - (portalPanelWidth + figurePanelWidth))));
                EditorGUILayout.LabelField(selectedFigure.GetExposableSpyroTagName(), boldStyle);
                EditorGUILayout.LabelField("Figure Index", selectedFigure.Index.ToString());
                EditorGUILayout.LabelField("Acknowledge State", selectedFigure.AcknowledgeState.ToString() + " (TODO)");
                EditorGUILayout.LabelField("Reading State", selectedFigure.ReadingState.ToString() + " (TODO)");
                EditorGUILayout.LabelField("TagHeader");
                EditorGUILayout.LabelField("\tBlock 0 Fetched", selectedFigure.headerBlockFetched[0].ToString());
                EditorGUILayout.LabelField("\tBlock 1 Fetched", selectedFigure.headerBlockFetched[1].ToString());
                if (selectedFigure.headerBlockFetched[0] && selectedFigure.headerBlockFetched[1])
                {
                    EditorGUILayout.Space();
                    unsafe
                    {
                        EditorGUILayout.LabelField("TagHeader Info", boldStyle);

                        ushort characterID = selectedFigure.TagHeader->toyType;
                        (ushort toyCode, VariantID variantID) = GetCharacterAndVariantIDs(selectedFigure);
                        bool foundMatch = SkylanderDatabase.GetSkylander(toyCode, out Skylander match);
                        EditorGUILayout.LabelField("CharacterID", foundMatch ? GetFriendlyToyCode(match) : GetFriendlyToyCode(toyCode));
                        EditorGUILayout.LabelField("Year Code", variantID.YearCode.ToString());
                        EditorGUILayout.LabelField("Is Repose", variantID.IsRepose.ToString());
                        EditorGUILayout.LabelField("Is Alt Deco", variantID.IsAltDeco.ToString());
                        EditorGUILayout.LabelField("Is LightCore", variantID.IsLightCore.ToString());
                        EditorGUILayout.LabelField("Is SuperCharger", variantID.IsSuperCharger.ToString());
                        EditorGUILayout.LabelField("DecoID", $"{variantID.DecoID} ({(DecoID)variantID.DecoID})");
                        EditorGUILayout.LabelField("Encoded VariantID", selectedFigure.TagHeader->subType.ToString());

                        ulong cardID = selectedFigure.TagHeader->tradingCardID;
                        EditorGUILayout.LabelField("WebCode", ValidTradingCardID(cardID) ? new string(Base29.EncodeWebCode(cardID)) : "(none)");

                        EditorGUILayout.Space();

                        SpyroTag_TagHeader header = Marshal.PtrToStructure<SpyroTag_TagHeader>((IntPtr)selectedFigure.TagHeader);
                        if (!headerFoldoutStates.ContainsKey(header))
                            headerFoldoutStates[header] = EditorPrefs.GetBool($"FoldoutState_{header.GetHashCode()}", true);

                        headerFoldoutStates[header] = EditorGUILayout.Foldout(headerFoldoutStates[header], "TagHeader Buffer", true);

                        if (headerFoldoutStates[header])
                        {
                            EditorGUILayout.BeginVertical("box");
                            foreach (string line in selectedFigure.TagHeader->ToString().Split('\n'))
                            {
                                EditorGUILayout.LabelField(line, bufferFont);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Info Discovery", boldStyle);
                        if (foundMatch)
                        {
                            EditorGUILayout.LabelField("Matching Skylander", match.name);
                            EditorGUILayout.LabelField("Type", match.Type.ToString());
                            EditorGUILayout.LabelField("Element", match.Element.ToString());
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Matching Variant", "(none) (TODO)");
                        }
                        else
                            EditorGUILayout.LabelField("Matching Skylander", "(none)");
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
                return;
            }

            if (selectedPortal == null)
            {
                EditorGUILayout.EndHorizontal();
                return;
            }

            // handle portal info printing
            infoScrollPos = EditorGUILayout.BeginScrollView(infoScrollPos, GUILayout.Width(position.width * (1 - (portalPanelWidth + figurePanelWidth))));
            EditorGUILayout.LabelField(selectedPortal.GetName(), boldStyle);
            EditorGUILayout.LabelField("Session ID", selectedPortal.SessionID.ToString());
            EditorGUILayout.LabelField("ID", BytesToHexString(selectedPortal.ID));
            EditorGUILayout.LabelField("DateTime Connected", selectedPortal.Timestamp.ToString());
            EditorGUILayout.LabelField("Active", selectedPortal.Active.ToString());
            EditorGUILayout.LabelField("Is Digital", selectedPortal.IsDigital.ToString());
            EditorGUILayout.Space();
            if (selectedPortal.GetPortalInfo())
            {
                PortalInfo info = selectedPortal.GetPortalInfo();
                EditorGUILayout.LabelField("Matching PortalInfo", info.name);
                EditorGUILayout.LabelField("Name", info.Name);
                EditorGUILayout.LabelField("LED Type", info.LEDType.ToString());
                EditorGUILayout.LabelField("Max Tags", info.MaxSimultaneousRFIDTags.ToString());
                EditorGUILayout.LabelField("Supported Commands", string.Join(" ", info.SupportedCommands));
                EditorGUILayout.LabelField("Additional Hardware", FlagsToString(info.AdditionalHardwarePieces));
            }
            else
                EditorGUILayout.LabelField("Matching PortalInfo", "(none)");

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        public void SetSelection(PortalOfPower portal)
        {
            selectedPortal = portal;
            figureInFocus = false;
            selectedFigure = null;
        }

        public void SetSelection(PortalFigure figure)
        {
            selectedFigure = figure;
            figureInFocus = true;
        }

        public static void Push(PortalOfPower portal)
        {
            activePortals.Add(portal);
        }

        public static void Pop(PortalOfPower portal)
        {
            activePortals.Remove(portal);

            if (selectedPortal != null && selectedPortal == portal)
                selectedPortal = null;

            if (selectedFigure?.Parent == portal)
                selectedFigure = null;
        }

        public static void Pop(PortalFigure figure)
        {
            if (selectedFigure != null && selectedFigure == figure)
            {
                selectedFigure = null;
                figureInFocus = false;
            }
        }
    }
}
#endif