using UnityEditor;

namespace PortalToUnity
{
    public class PortalInfoCreatorWindow : EditorWindow
    {
        public static void ShowWindow(string newName)
        {
            PortalInfoCreatorWindow window = GetWindow<PortalInfoCreatorWindow>("Create Portal Info");
            //window.workingSkylander = CreateInstance<Skylander>();
            //window.workingSkylander.Name = newName;
            window.Show();
        }
    }
}