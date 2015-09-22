using System;
using System.Collections.Generic;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Multimedia.Audio.Project;
using UniversalEditor.ObjectModels.Markup;
namespace UniversalEditor.DataFormats.Multimedia.Audio.Project.LMMS
{
	public class LMMSProjectDataFormat : XMLDataFormat
	{
		protected override DataFormatReference MakeReferenceInternal()
		{
			DataFormatReference dfr = base.MakeReferenceInternal();
			dfr.Clear();
			dfr.Capabilities.Add(typeof(AudioProjectObjectModel), DataFormatCapabilities.All);
			return dfr;
		}
		protected override void BeforeLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeLoadInternal(objectModels);
			MarkupObjectModel mom = new MarkupObjectModel();
			objectModels.Push(mom);
		}
		protected override void AfterLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.AfterLoadInternal(objectModels);
			MarkupObjectModel mom = objectModels.Pop() as MarkupObjectModel;
			AudioProjectObjectModel seq = objectModels.Pop() as AudioProjectObjectModel;
		}
	}
}