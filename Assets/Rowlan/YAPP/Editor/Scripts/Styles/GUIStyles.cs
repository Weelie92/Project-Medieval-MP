using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    public class GUIStyles
    {

        private static GUIStyle _appTitleBoxStyle;
        public static GUIStyle AppTitleBoxStyle
        {
            get
            {
                if (_appTitleBoxStyle == null)
                {
                    _appTitleBoxStyle = new GUIStyle("helpBox");
                    _appTitleBoxStyle.fontStyle = FontStyle.Bold;
                    _appTitleBoxStyle.fontSize = 16;
                    _appTitleBoxStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _appTitleBoxStyle;
            }
        }

        private static GUIStyle _boxTitleStyle;
        public static GUIStyle BoxTitleStyle
        {
            get
            {
                if (_boxTitleStyle == null)
                {
                    _boxTitleStyle = new GUIStyle("Label");
                    _boxTitleStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                return _boxTitleStyle;
            }
        }

        private static GUIStyle _helpBoxStyle;
        public static GUIStyle HelpBoxStyle
        {
            get
            {
                if (_helpBoxStyle == null)
                {
                    _helpBoxStyle = new GUIStyle("helpBox");
                    _helpBoxStyle.fontStyle = FontStyle.Bold;
                }
                return _helpBoxStyle;
            }
        }

        private static GUIStyle _groupTitleStyle;
        public static GUIStyle GroupTitleStyle
        {
            get
            {
                if (_groupTitleStyle == null)
                {
                    _groupTitleStyle = new GUIStyle("Label");
                    _groupTitleStyle.fontStyle = FontStyle.Bold;
                }
                return _groupTitleStyle;
            }
        }

        private static GUIStyle _dropAreaStyle;
        public static GUIStyle DropAreaStyle
        {
            get
            {
                if (_dropAreaStyle == null)
                {
                    _dropAreaStyle = new GUIStyle("box");
                    _dropAreaStyle.fontStyle = FontStyle.Italic;
                    _dropAreaStyle.alignment = TextAnchor.MiddleCenter;
                    _dropAreaStyle.normal.textColor = GUI.skin.label.normal.textColor;
                }
                return _dropAreaStyle;
            }
        }

        private static GUIStyle _separatorStyle;
        public static GUIStyle SeparatorStyle
        {
            get
            {
                if (_separatorStyle == null)
                {
                    _separatorStyle = new GUIStyle("box");
                    _separatorStyle.normal.background = CreateColorPixel(Color.gray);
                    _separatorStyle.stretchWidth = true;
                    _separatorStyle.border = new RectOffset(0, 0, 0, 0);
                    _separatorStyle.fixedHeight = 1f;
                }
                return _separatorStyle;
            }
        }

        /// <summary>
        /// Used for include, exclude and unselected texture border
        /// </summary>
        private static GUIStyle _textureSelectionStyleUnselected;
        public static GUIStyle TextureSelectionStyleUnselected
        {
            get
            {
                if (_textureSelectionStyleUnselected == null)
                {
                    _textureSelectionStyleUnselected = new GUIStyle("label");
                    _textureSelectionStyleUnselected.normal.background = CreateColorPixel(Color.gray);
                    _textureSelectionStyleUnselected.stretchWidth = true;
                    _textureSelectionStyleUnselected.border = new RectOffset(1, 1, 1, 1);
                }
                return _textureSelectionStyleUnselected;
            }
        }

        private static GUIStyle _textureSelectionStyleInclude;
        public static GUIStyle TextureSelectionStyleInclude
        {
            get
            {
                if (_textureSelectionStyleInclude == null)
                {
                    _textureSelectionStyleInclude = new GUIStyle(TextureSelectionStyleUnselected);
                    _textureSelectionStyleInclude.normal.background = CreateColorPixel(Color.green);
                }
                return _textureSelectionStyleInclude;
            }
        }

        private static GUIStyle _textureSelectionStyleExclude;
        public static GUIStyle TextureSelectionStyleExclude
        {
            get
            {
                if (_textureSelectionStyleExclude == null)
                {
                    _textureSelectionStyleExclude = new GUIStyle(TextureSelectionStyleUnselected);
                    _textureSelectionStyleExclude.normal.background = CreateColorPixel(Color.red);
                }
                return _textureSelectionStyleExclude;
            }
        }

        /// <summary>
        /// Creates a 1x1 texture
        /// </summary>
        /// <param name="Background">Color of the texture</param>
        /// <returns></returns>
        public static Texture2D CreateColorPixel(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static Color DefaultBackgroundColor = GUI.backgroundColor;
        public static Color ErrorBackgroundColor = new Color( 1f,0f,0f,0.7f); // red tone

        public static Color BrushNoneInnerColor = new Color(0f, 0f, 1f, 0.05f); // blue tone
        public static Color BrushNoneOuterColor = new Color(0f, 0f, 1f, 1f); // blue tone

        public static Color BrushAddInnerColor = new Color(0f, 1f, 0f, 0.05f); // green tone
        public static Color BrushAddOuterColor = new Color(0f, 1f, 0f, 1f); // green tone

        public static Color BrushRemoveInnerColor = new Color(1f, 0f, 0f, 0.05f); // red tone
        public static Color BrushRemoveOuterColor = new Color(1f, 0f, 0f, 1f); // red tone

        public static Color DropAreaBackgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f); // gray tone

        public static Color PhysicsRunningButtonBackgroundColor = new Color(1f, 0f, 0f, 0.7f); // red tone

    }
}