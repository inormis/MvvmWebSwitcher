using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;
using WebSwitcher.Configurations;

namespace WebSwitcher.IconFiles {
    public class CSharpSwitchButton : SwitchButton {
        private MethodDeclarationSyntax _lastActiveAction;

        public CSharpSwitchButton(IDocumentContext view) : base(view, "CSharp.jpg") { }

        protected override string GetPathPattern(IConfiguration configuration) {
            var root = ConvertRootForPathPattern(configuration.CSharpRootPath);
            return $@"\\{root}((?<Module>\S*)\\)?(?<Controller>\S*)Controller\.cs$";
        }

        protected override string GetFilePathFromPattern(FilePattern pattern, IConfiguration configuration) {
            return Path.Combine(pattern.RootDirectory,
                $@"{configuration.CSharpRootPath}{pattern.ModulePath}{pattern.Controller}Controller.cs");
        }

        protected override FilePattern CreateSwitchPattern(string filePath, string action, string controller,
            string module,
            string rootDirectory) {
            var currentOrFirstAction = _lastActiveAction ?? GetFirstActionInController();
            return base.CreateSwitchPattern(filePath, currentOrFirstAction?.Identifier.Text, controller, module, rootDirectory);
        }

        private MethodDeclarationSyntax GetFirstActionInController() {
            var caretPosition = Context.TextView.Caret.Position.BufferPosition;
            var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            var syntaxNode = document.GetSyntaxRootAsync().Result;
            return syntaxNode.SyntaxTree.GetRoot().DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(IsActionMethod);
        }

        public override void OnOpened(FilePattern switchContextPattern) {
            var actionNode = GetActionNodeFromName(switchContextPattern.Action);
            if (actionNode != null)
                ((TextSelection) Context.Document.Selection).MoveToAbsoluteOffset(actionNode.SpanStart);
        }

        public override void Activate() {
            base.Activate();
            Context.TextView.Caret.PositionChanged += OnPositionChanged;
        }

        public override void Deactivate() {
            base.Deactivate();
            Context.TextView.Caret.PositionChanged -= OnPositionChanged;
        }

        private void OnPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            if (_lastActiveAction != null) {
                var currentPosition = e.NewPosition.BufferPosition.Position;
                if (IsPositionWithinLastActiveActionMethod(currentPosition)) return;
            }
            var activeActionOrNull = GetCurrentActionFromCarretOrNull();
            _lastActiveAction = activeActionOrNull ?? _lastActiveAction;
        }

        private bool IsPositionWithinLastActiveActionMethod(int currentPosition) {
            return _lastActiveAction.Span.Start <= currentPosition &&
                   currentPosition <= _lastActiveAction.Span.End;
        }

        private MethodDeclarationSyntax GetActionNodeFromName(string action) {
            var caretPosition = Context.TextView.Caret.Position.BufferPosition;

            var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (!document.TryGetSyntaxTree(out var tree))
                return null;

            var actionNodeFromName = tree.GetRoot().DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(x =>
                    string.Compare(x.Identifier.Text, action, StringComparison.InvariantCultureIgnoreCase) ==
                    0);
            return actionNodeFromName;
        }

        private MethodDeclarationSyntax GetCurrentActionFromCarretOrNull() {
            var caretPosition = Context.TextView.Caret.Position.BufferPosition;
            var document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document == null)
                return null;

            var syntaxNode = document.GetSyntaxRootAsync().Result;
            var node = syntaxNode.FindToken(caretPosition).Parent
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
            if (node == null)
                return null;

            return IsActionMethod(node) ? node : null;
        }

        private static bool IsActionMethod(MethodDeclarationSyntax node) {
            var isPublic = node.Modifiers.Any(x => x.Text == "public");
            var returnTypeIsActionResult = GetReturnTypeNameOrNull(node)?
                                               .EndsWith("Result", StringComparison.InvariantCultureIgnoreCase) == true;
            var isActionMethod = isPublic && returnTypeIsActionResult;
            return isActionMethod;
        }

        private static string GetReturnTypeNameOrNull(MethodDeclarationSyntax node) {
            return (node.ReturnType as IdentifierNameSyntax)?.Identifier.Text;
        }
    }
}