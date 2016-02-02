using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace VSPathToClipboard
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidVsPtcPkgString)]
    public sealed class PathToClipboardPackage : Package
    {
        private DTE _dte;

        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetGlobalService(typeof(DTE)) as DTE;
            if (_dte == null)
                return;

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null)
                return;

            //Solution Node
            CommandID solutionMenuCommandId = new CommandID(GuidList.GuidVsPtcCmdSet, (int)PkgCmdIdList.CmdidMyCommandSolution);
            MenuCommand solutionmMenuItem = new MenuCommand(SolutionMenuCallback, solutionMenuCommandId);
            mcs.AddCommand(solutionmMenuItem);

            //Project Node
            CommandID projectMenuCommandId = new CommandID(GuidList.GuidVsPtcCmdSet, (int)PkgCmdIdList.CmdidMyCommandProject);
            MenuCommand projectmMenuItem = new MenuCommand(ProjectMenuCallback, projectMenuCommandId);
            mcs.AddCommand(projectmMenuItem);

            //Project Item Node - File
            CommandID itemFileMenuCommandId = new CommandID(GuidList.GuidVsPtcCmdSet, (int)PkgCmdIdList.CmdidMyCommandItemFile);
            MenuCommand itemFileMenuItem = new MenuCommand(ItemFileMenuCallback, itemFileMenuCommandId);
            mcs.AddCommand(itemFileMenuItem);

            //Project Item Node - Folder
            CommandID itemFolderMenuCommandId = new CommandID(GuidList.GuidVsPtcCmdSet, (int)PkgCmdIdList.CmdidMyCommandItemFolder);
            MenuCommand itemFolderMenuItem = new MenuCommand(ItemFolderMenuCallback, itemFolderMenuCommandId);
            mcs.AddCommand(itemFolderMenuItem);
        }

        private async void SolutionMenuCallback(object sender, EventArgs e)
        {
            if (_dte.SelectedItems.Count != 1) return;

            Solution solution = _dte.Solution;

            var path = Path.GetDirectoryName(solution.FullName);

            if (path == null)
                await ShowMessage("Error getting path!");
            else
            {
                bool sent = SendToClipboard(path);
                if (sent)
                    await ShowMessage("Copied!");
                else
                    await ShowMessage("Error sending to clipboard!");
            }

            _dte.StatusBar.Clear();
        }

        private async void ProjectMenuCallback(object sender, EventArgs e)
        {
            if (_dte.SelectedItems.Count != 1) return;

            SelectedItem item = _dte.SelectedItems.Item(1);
            Project project = item.Project;

            var path = Path.GetDirectoryName(project.FullName);

            if (path == null)
                await ShowMessage("Error getting path!");
            else
            {
                bool sent = SendToClipboard(path);
                if (sent)
                    await ShowMessage("Copied!");
                else
                    await ShowMessage("Error sending to clipboard!");
            }

            _dte.StatusBar.Clear();
        }

        private async void ItemFileMenuCallback(object sender, EventArgs e)
        {
            if (_dte.SelectedItems.Count != 1) return;

            SelectedItem item = _dte.SelectedItems.Item(1);

            string name = item.ProjectItem.FileNames[1];

            bool sent = SendToClipboard(name);
            if (sent)
                await ShowMessage("Copied!");
            else
                await ShowMessage("Error sending to clipboard!");

            _dte.StatusBar.Clear();
        }

        private async void ItemFolderMenuCallback(object sender, EventArgs e)
        {
            if (_dte.SelectedItems.Count != 1) return;

            SelectedItem item = _dte.SelectedItems.Item(1);

            string path = Path.GetDirectoryName(item.ProjectItem.FileNames[1]);

            if (path == null)
                await ShowMessage("Error getting path!");
            else
            {
                bool sent = SendToClipboard(path);
                if (sent)
                    await ShowMessage("Copied!");
                else
                    await ShowMessage("Error sending to clipboard!");
            }

            _dte.StatusBar.Clear();
        }

        private async Task ShowMessage(string message)
        {
            _dte.StatusBar.Text = message;
            await Task.Delay(1000);
        }

        private bool SendToClipboard(string value)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
