﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeAnalysis.Text;

namespace dnSpy.Roslyn.Internal.SmartIndent {
	/// <summary>
	/// An indentation result represents where the indent should be placed.  It conveys this through
	/// a pair of values.  A position in the existing document where the indent should be relative,
	/// and the number of columns after that the indent should be placed at.
	///
	/// This pairing provides flexibility to the implementor to compute the indentation results in
	/// a variety of ways.  For example, one implementation may wish to express indentation of a
	/// newline as being four columns past the start of the first token on a previous line.  Another
	/// may wish to simply express the indentation as an absolute amount from the start of the
	/// current line.  With this tuple, both forms can be expressed, and the implementor does not
	/// have to convert from one to the other.
	/// </summary>
	readonly struct IndentationResult {
		/// <summary>
		/// The base position in the document that the indent should be relative to.  This position
		/// can occur on any line (including the current line, or a previous line).
		/// </summary>
		public int BasePosition { get; }

		/// <summary>
		/// The number of columns the indent should be at relative to the BasePosition's column.
		/// </summary>
		public int Offset { get; }

		public IndentationResult(int basePosition, int offset) : this() {
			this.BasePosition = basePosition;
			this.Offset = offset;
		}
	}

	internal static class IndentationResultExtensions {
		public static string GetIndentationString(this IndentationResult indentationResult, SourceText sourceText, bool useTabs, int tabSize) {
			var baseLine = sourceText.Lines.GetLineFromPosition(indentationResult.BasePosition);
			var baseOffsetInLine = indentationResult.BasePosition - baseLine.Start;

			var indent = baseOffsetInLine + indentationResult.Offset;

			var indentString = indent.CreateIndentationString(useTabs, tabSize);
			return indentString;
		}

		public static string CreateIndentationString(this int desiredIndentation, bool useTab, int tabSize) {
			var numberOfTabs = 0;
			var numberOfSpaces = Math.Max(0, desiredIndentation);

			if (useTab) {
				numberOfTabs = desiredIndentation / tabSize;
				numberOfSpaces -= numberOfTabs * tabSize;
			}

			return new string('\t', numberOfTabs) + new string(' ', numberOfSpaces);
		}
	}
}
