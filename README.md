StyleCop Community Rules
========================

StyleCop Community Rules contain additional rules for [Microsoft StyleCop](http://code.msdn.microsoft.com/sourceanalysis).

Rules
-----

The following rules are available:

* _LinesMustNotEndWithWhitespace_ (CO1000): Checks for trailing whitespace
* _LinesMustNotBeLongerThanNumCharacters_ (CO1001): Checks that line length does not exceed a configured number of characters
* _FilesMustNotEndWithMultipleEmptyLines_ (CO1002): Checks for multiple empty lines at the end of file
* _FilesMustEndWithNewline_ (CO1003): Checks that a file ends with a newline
* _FilesMustNotStartWithWhitespace_ (CO1004): Checks that file does not start with whitespace

Building the Sources
--------------------

StyleCop Community Rules are developed with Microsoft Visual Studio 2008. Building the project with the Express Edition should also work.

Copy _Microsoft.StyleCop.dll_ and _Microsoft.StyleCop.CSharp.dll_ from the StyleCop installation directory into this project's Libraries directory. Then open the solution _Community.StyleCop.Rules.sln_ in Visual Studio and build it. The resulting DLL is called _Community.StyleCop.CSharp.Rules.dll_.

Installation
------------

StyleCop Community Rules require at least Microsoft .NET Framework 3.5.

Copy _Community.StyleCop.CSharp.Rules.dll_ into the StyleCop installation directory. StyleCop will pick up the new rules automatically and use them on its next run.

Configuration
-------------

You can configure the rules through StyleCop Settings in Microsoft Visual Studio. Some settings are exposed directly in the _Rules_ tab of StyleCop Settings, others are in the _Community Settings_ tab.

Bugs
----

If you find bugs in this software, please [create an issue](http://github.com/schlotter/Community.StyleCop.Rules/issues) or [contact me directly](again@gmx.de). Of course, patches and forks are also welcome!

License
-------

StyleCop Community Rules is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

StyleCop Community Rules is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with StyleCop Community Rules. If not, see <http://www.gnu.org/licenses/>.
