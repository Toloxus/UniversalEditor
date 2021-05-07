﻿//
//  IcarusCommandUse.cs - represents the ICARUS "use" command
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

using UniversalEditor.ObjectModels.Icarus.Parameters;

namespace UniversalEditor.ObjectModels.Icarus.Commands
{
	/// <summary>
	/// Represents the ICARUS "use" command.
	/// </summary>
	public class IcarusCommandUse : IcarusPredefinedCommand
	{
		public IcarusCommandUse()
		{
			Parameters.Add(new IcarusGenericParameter("Target"));
		}

		public override string Name { get { return "use"; } }

		public IcarusExpression Target { get { return Parameters["Target"].Value; } set { Parameters["Target"].Value = value; } }

		public override object Clone()
		{
			IcarusCommandUse command = new IcarusCommandUse();
			command.Target = (Target.Clone() as IcarusExpression);
			return command;
		}
	}
}
