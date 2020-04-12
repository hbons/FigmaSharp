﻿/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using AppKit;
using System;
using System.Linq;
using System.Text;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class ComboBoxConverter : FigmaNativeControlConverter
	{
		public override Type GetControlType(FigmaNode currentNode)
		{
			return typeof(NSComboBox);
		}

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.ComboBox;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var view = new NSComboBox ();
			var figmaInstance = (FigmaFrameEntity)currentNode;
			view.Configure (currentNode);

			figmaInstance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.ComboBoxSmall:
				case NativeControlComponentType.ComboBoxSmallDark:
					view.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.ComboBoxStandard:
				case NativeControlComponentType.ComboBoxStandardDark:
					view.ControlSize = NSControlSize.Regular;
					view.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					break;
			}

			var label = figmaInstance.children
			   .OfType<FigmaText> ()
			   .FirstOrDefault (s => s.name == "lbl");

			if (label != null && !string.IsNullOrEmpty (label.characters)) {
				view.StringValue = label.characters;
			}

			return new View (view);
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode.Node;

			var builder = new StringBuilder ();
			var name = Resources.Ids.Conversion.NameIdentifier;

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				builder.WriteConstructor (name, GetControlType (currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			builder.Configure (currentNode.Node, name);

			figmaInstance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.ComboBoxSmall:
				case NativeControlComponentType.ComboBoxSmallDark:
					builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Small);
					break;
				case NativeControlComponentType.ComboBoxStandard:
				case NativeControlComponentType.ComboBoxStandardDark:
					builder.WriteEquality(currentNode.Name, nameof(NSButton.Font),
					    CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
					break;
			}

			var label = figmaInstance.children
				.OfType<FigmaText> ()
				.FirstOrDefault (s => s.name == "lbl");

			if (label != null && !string.IsNullOrEmpty (label.characters)) {
				var textLabel = NativeControlHelper.GetTranslatableString(label.characters, rendererService.CurrentRendererOptions.TranslateLabels);
				if (!rendererService.CurrentRendererOptions.TranslateLabels) {
					textLabel = $"\"{textLabel}\"";
				}
				var nsstringcontructor = typeof (Foundation.NSString).GetConstructor (new[] { textLabel });
				builder.WriteMethod (name, nameof (NSComboBox.Add), nsstringcontructor);
			}
			//if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
			//	builder.AppendLine (string.Format ("{0}.Appearance = NSAppearance.GetAppearance ({1});", name, NSAppearance.NameDarkAqua.GetType ().FullName));
			//}

			return builder;
		}
	}
}
