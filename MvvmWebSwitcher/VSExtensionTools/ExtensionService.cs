using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebSwitcher.VSExtensionTools {
    public static class ExtensionService {
        private static readonly Lazy<IVsSolution> _vsSolution = new Lazy<IVsSolution>(GetService<IVsSolution>);

        private static readonly Lazy<Solution> _solution = new Lazy<Solution>(GetSolution);

        public static IVsSolution VsSolution => _vsSolution.Value;

        public static Solution Solution => _solution.Value;

        private static Solution GetSolution() {
            return GetService<DTE>().Solution;
        }

        public static T GetService<T>() {
            return (T) Package.GetGlobalService(typeof(T));
        }

        public static IEnumerable<Project> GetAllProjects() {
            return GetProjects(VsSolution);
        }

        private static IEnumerable<Project> GetProjects(IVsSolution solution) {
            foreach (var hier in GetProjectsInSolution(solution)) {
                var project = GetDTEProject(hier);
                if (project != null)
                    yield return project;
            }
        }

        private static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags = __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION) {
            if (solution == null)
                yield break;

            var guid = Guid.Empty;
            solution.GetProjectEnum((uint) flags, ref guid, out var enumHierarchies);
            if (enumHierarchies == null)
                yield break;

            var hierarchy = new IVsHierarchy[1];
            while (enumHierarchies.Next(1, hierarchy, out var fetched) == VSConstants.S_OK && fetched == 1)
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
        }

        private static Project GetDTEProject(IVsHierarchy hierarchy) {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) __VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as Project;
        }
    }
}