using System;

using UniversalEditor.IO;
using UniversalEditor.ObjectModels.Multimedia3D.Model;

namespace UniversalEditor.DataFormats.Multimedia3D.Model.ThreeDStudio
{
	public class ThreeDSMaxDataFormat : DataFormat
	{
		private static DataFormatReference _dfr = null;
		protected override DataFormatReference MakeReferenceInternal()
		{
			if (_dfr == null)
			{
				_dfr = base.MakeReferenceInternal();
				_dfr.Capabilities.Add(typeof(ModelObjectModel), DataFormatCapabilities.All);
			}
			return _dfr;
		}
		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			ModelObjectModel model = (objectModel as ModelObjectModel);
			if (model == null) throw new ObjectModelNotSupportedException();

			Reader reader = base.Accessor.Reader;
			throw new NotImplementedException();
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			ModelObjectModel model = (objectModel as ModelObjectModel);
			if (model == null) throw new ObjectModelNotSupportedException();

			Writer reader = base.Accessor.Writer;
			throw new NotImplementedException();
		}
	}
}