using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace UniversalEditor.UserInterface.Pages
{
	public partial class FilePage : Page
	{
		private Document mvarDocument = null;
		public Document Document
		{
			get { return mvarDocument; }
			set
			{
				if (mvarDocument == value)
					return;

				OnDocumentChanging(EventArgs.Empty);

				mvarDocument = value;
				if (mvarDocument.Accessor != null)
				{
					try
					{
						Title = System.IO.Path.GetFileName(mvarDocument.Accessor.GetFileName());
					}
					catch
					{
						Title = mvarDocument.Accessor.GetFileName();
					}
					Description = mvarDocument.Accessor.GetFileName();
				}

				OnDocumentChanged(EventArgs.Empty);
			}
		}

		protected virtual void OnDocumentChanging(EventArgs e)
		{
		}
		protected virtual void OnDocumentChanged(EventArgs e)
		{
		}
	}
}
