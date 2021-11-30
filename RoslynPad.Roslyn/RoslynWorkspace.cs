// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;

namespace RoslynPad.Roslyn
{
    public class RoslynWorkspace : Workspace
    {
        public DocumentId? OpenDocumentId { get; private set; }
        public RoslynHost? RoslynHost { get; }

        public RoslynWorkspace(HostServices hostServices, string workspaceKind = WorkspaceKind.Host, RoslynHost? roslynHost = null)
            : base(hostServices, workspaceKind)
        {
            DiagnosticProvider.Enable(this, DiagnosticProvider.Options.Semantic);

            RoslynHost = roslynHost;
        }

        public new Task SetCurrentSolution(Solution solution)
        {
            var oldSolution = CurrentSolution;
            var newSolution = base.SetCurrentSolution(solution);
            return RaiseWorkspaceChangedEventAsync(WorkspaceChangeKind.SolutionChanged, oldSolution, newSolution);
        }

        public override bool CanOpenDocuments => true;

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
	        return feature switch
	        {
		        ApplyChangesKind.ChangeDocument => true,
		        ApplyChangesKind.ChangeDocumentInfo => true,
		        ApplyChangesKind.AddMetadataReference => true,
		        ApplyChangesKind.RemoveMetadataReference => true,
		        ApplyChangesKind.AddAnalyzerReference => true,
		        ApplyChangesKind.RemoveAnalyzerReference => true,
		        _ => false
	        };
        }

        public void OpenDocument(DocumentId documentId, SourceTextContainer textContainer)
        {
            OpenDocumentId = documentId;
            OnDocumentOpened(documentId, textContainer);
            OnDocumentContextUpdated(documentId);
        }

        public event Action<DocumentId, SourceText>? ApplyingTextChange;

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);

            ApplyingTextChange = null;

            DiagnosticProvider.Disable(this);
        }

        protected override void ApplyDocumentTextChanged(DocumentId document, SourceText newText)
        {
            if (OpenDocumentId != document)
            {
                return;
            }

            ApplyingTextChange?.Invoke(document, newText);

            OnDocumentTextChanged(document, newText, PreservationMode.PreserveIdentity);
        }
    }
}
