//
//  View.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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
using System;
using MBS.Framework.UserInterface;

namespace UniversalEditor.UserInterface
{
	public abstract class View : Container
	{
		public Editor Editor { get; set; }

		private ObjectModel _ObjectModel = null;
		public ObjectModel ObjectModel
		{
			get { return _ObjectModel; }
			set
			{
				if (_ObjectModel == value) return;

				_ObjectModel = value;
				OnObjectModelChanged(EventArgs.Empty);
			}
		}

		protected virtual void OnObjectModelChanged(EventArgs e)
		{

		}
	}
}
