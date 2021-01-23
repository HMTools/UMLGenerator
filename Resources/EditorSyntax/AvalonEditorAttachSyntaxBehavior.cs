using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UMLGenerator.Resources.EditorSyntax
{
    public class AvalonEditorAttachSyntaxBehavior : Behavior<TextEditor>
    {

        public static readonly DependencyProperty SyntaxProperty =
        DependencyProperty.Register("Syntax", typeof(IHighlightingDefinition), typeof(AvalonEditorAttachSyntaxBehavior),
        new FrameworkPropertyMetadata(default(IHighlightingDefinition), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SyntaxPropertyChangedCallback));

        public IHighlightingDefinition Syntax
        {
            get { return (IHighlightingDefinition)GetValue(SyntaxProperty); }
            set { SetValue(SyntaxProperty, value); }
        }

        public static readonly DependencyProperty BindableTextProperty =
        DependencyProperty.Register("BindableText", typeof(string), typeof(AvalonEditorAttachSyntaxBehavior),
        new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, BindableTextPropertyChangedCallback));
        public string BindableText
        {
            get { return (string)GetValue(BindableTextProperty); }
            set { SetValue(BindableTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.SyntaxHighlighting = Syntax;
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    BindableText = textEditor.Document.Text;
            }
        }

        private static void BindableTextPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditorAttachSyntaxBehavior;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject;
                if (editor.Document != null)
                {
                    var caretOffset = editor.CaretOffset;
                    editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                    editor.CaretOffset = caretOffset;
                }
            }
        }
        private static void SyntaxPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditorAttachSyntaxBehavior;
            if (behavior.AssociatedObject != null)
            {
                behavior.AssociatedObject.SyntaxHighlighting = dependencyPropertyChangedEventArgs.NewValue as IHighlightingDefinition;
            }
        }

    }
}
