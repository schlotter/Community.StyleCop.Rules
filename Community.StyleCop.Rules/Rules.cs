// <copyright file="Rules.cs" company="n/a">
// Copyright 2009 Christian Schlotter
//
// This file is part of StyleCop Community Rules.
//
// StyleCop Community Rules is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or (at your
// option) any later version.
//
// StyleCop Community Rules is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with StyleCop Community Rules. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <author>Christian Schlotter</author>
// <email>again@gmx.de</email>
// <summary>Contains enumeration that represents the rules.</summary>

namespace Community.StyleCop.CSharp
{
    /// <summary>
    /// Enumeration that represents the rules offered by the analyzers.
    /// </summary>
    internal enum Rules
    {
        /// <summary>
        /// Represents the rule that checks line length.
        /// </summary>
        LinesMustNotBeLongerThanNumCharacters,

        /// <summary>
        /// Represents the rule that checks for trailing whitespace.
        /// </summary>
        LinesMustNotEndWithWhitespace,

        /// <summary>
        /// Represents the rule that checks for empty lines at the end of file.
        /// </summary>
        FilesMustNotEndWithMultipleEmptyLines,

        /// <summary>
        /// Represents the rule that checks for whitespace at the beginning of
        /// file.
        /// </summary>
        FilesMustNotStartWithWhitespace,

        /// <summary>
        /// Represents the rule that checks if a file ends with a newline.
        /// </summary>
        FilesMustEndWithNewline
    }
}
