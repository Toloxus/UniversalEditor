//
//  FDSEQDataFormat.cs - provides a DataFormat to manipulate FixedDocumentSequence files in a Microsoft XML Paper Specification (XPS) document
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2011-2020 Mike Becker's Software
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.ObjectModels.Text.Formatted.XPS.FixedDocumentSequence;

namespace UniversalEditor.DataFormats.Text.Formatted.XPS.FixedDocumentSequence
{
	/// <summary>
	/// Provides a <see cref="DataFormat" /> to manipulate FixedDocumentSequence files in a Microsoft XML Paper Specification (XPS) document.
	/// </summary>
	public class FDSEQDataFormat : XMLDataFormat
	{
		protected override void BeforeLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeLoadInternal(objectModels);
			objectModels.Push(new MarkupObjectModel());
		}
		protected override void AfterLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.AfterLoadInternal(objectModels);

			MarkupObjectModel mom = (objectModels.Pop() as MarkupObjectModel);
			FixedDocumentSequenceObjectModel fdseq = (objectModels.Pop() as FixedDocumentSequenceObjectModel);

			MarkupTagElement tagFixedDocumentSequence = (mom.Elements["FixedDocumentSequence"] as MarkupTagElement);
			foreach (MarkupElement elDocumentReference in tagFixedDocumentSequence.Elements)
			{
				MarkupTagElement tagDocumentReference = (elDocumentReference as MarkupTagElement);
				if (tagDocumentReference == null) continue;
				if (tagDocumentReference.FullName != "DocumentReference") continue;

				MarkupAttribute attSource = tagDocumentReference.Attributes["Source"];
				if (attSource == null) continue;

				DocumentReference docref = new DocumentReference();
				docref.Source = attSource.Value;

				fdseq.DocumentReferences.Add(docref);
			}
		}

		public XPSSchemaVersion SchemaVersion { get; set; } = XPSSchemaVersion.OpenXPS;

		protected override void BeforeSaveInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeSaveInternal(objectModels);

			FixedDocumentSequenceObjectModel fdseq = (objectModels.Pop() as FixedDocumentSequenceObjectModel);
			MarkupObjectModel mom = new MarkupObjectModel();

			mom.Elements.Add(new MarkupPreprocessorElement("xml", "version=\"1.0\" encoding=\"UTF-8\""));

			MarkupTagElement tagFixedDocumentSequence = new MarkupTagElement() { Name = "FixedDocumentSequence" };
			tagFixedDocumentSequence.XMLSchema = XPSSchemas.GetSchema(SchemaVersion, XPSSchemaType.FixedDocumentSequence);

			foreach (DocumentReference docref in fdseq.DocumentReferences)
			{
				MarkupTagElement tagDocumentReference = new MarkupTagElement() { Name = "DocumentReference" };
				tagDocumentReference.Attributes.Add("Source", docref.Source);
				tagFixedDocumentSequence.Elements.Add(tagDocumentReference);
			}

			mom.Elements.Add(tagFixedDocumentSequence);
			objectModels.Push(mom);
		}
	}
}
