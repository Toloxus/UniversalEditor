using System;
using UniversalEditor.ObjectModels.Multimedia3D.Model;

namespace UniversalEditor.DataFormats.Multimedia3D.Model.Metasequoia
{
	public class MQOTextDataFormat : DataFormat
	{
        public override DataFormatReference MakeReference()
        {
            DataFormatReference dfr = base.MakeReference();
            dfr.Filters.Add("Metasequoia model", new string[] { "*.mqo" });
            dfr.Capabilities.Add(typeof(ModelObjectModel), DataFormatCapabilities.All);
            return dfr;
        }

		private float mvarFormatVersion = 1.0f;
		public float FormatVersion { get { return mvarFormatVersion; } set { mvarFormatVersion = value; } }

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			ModelObjectModel model = (objectModel as ModelObjectModel);
			if (model == null) throw new ObjectModelNotSupportedException();

			IO.Reader tr = base.Accessor.Reader;
			string MetasequoiaDocument = tr.ReadLine();
			if (MetasequoiaDocument != "MetasequoiaDocument") throw new InvalidDataFormatException("File does not begin with \"MetasequoiaDocument\"");

			string DocumentFormat = tr.ReadLine();
			if (!DocumentFormat.StartsWith("Format Text Ver ")) throw new InvalidDataFormatException("Cannot understand Metasequoia format \"" + DocumentFormat + "\"");

			mvarFormatVersion = Single.Parse(DocumentFormat.Substring(16));

			while (!tr.EndOfStream)
			{
				string line = tr.ReadLine();
				if (String.IsNullOrEmpty(line.Trim())) continue;

				if (line.StartsWith("Scene "))
				{
				}
				else if (line.StartsWith("Material "))
				{
					string materialCountStr = line.Substring(9, line.IndexOf("{") - 9);
					int materialCount = Int32.Parse(materialCountStr);
				}
			}
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
            ModelObjectModel model = (objectModel as ModelObjectModel);
            if (model == null) throw new ObjectModelNotSupportedException();

			IO.Writer tw = base.Accessor.Writer;
			tw.WriteLine("MetasequoiaDocument");

			tw.WriteLine("Format Text Ver " + mvarFormatVersion.ToString("0.#"));

			tw.WriteLine("Material " + model.Materials.Count.ToString());
			foreach (ModelMaterial mat in model.Materials)
			{
			}
		}
	}
}