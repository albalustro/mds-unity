using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;

namespace Playmove
{
    [XmlRoot("KeyboardConfiguration")]
    public class PKKeyboardLettersLoader
    {
        public PKKeyboardLettersLoader() { }

        public PKKeyboardLettersLoader(TextAsset configFile)
        {
            PKKeyboardLettersLoader config = PYXML.DeserializerFromContent<PKKeyboardLettersLoader>(configFile.text);
            Confirm = config.Confirm;
            Cancel = config.Cancel;
            Backspace = config.Backspace;
            ClearAll = config.ClearAll;
            Pages = config.Pages;
        }

        public struct Page
        {
            [XmlArrayItem("Line")]
            public string[] Lines;
            public string SpecialLetter;
        }

        public Page[] Pages;

        public string Confirm;
        public string Cancel;
        public string Backspace;
        public string ClearAll;

        //public void CreateTest()
        //{
        //    //PYXML.Serializer(Application.dataPath + "/test.xml", new PKKeyboardLettersLoader()
        //    //{
        //    //    Backspace = "aasd",
        //    //    Cancel = "aasd",
        //    //    ClearAll = "aasd",
        //    //    Confirm = "aasd",
        //    //    Pages = new PKKeyboardLettersLoader.Page[] {
        //    //        new PKKeyboardLettersLoader.Page() { Lines = new string[] { "Q W E R T", "A S D F" } },
        //    //        new PKKeyboardLettersLoader.Page() { Lines = new string[] { "1 2 3", "4 5 6" } }
        //    //    },
        //    //    SpecialKeys = new string[] { "adasd", "123132" }
        //    //});
        //}

        public string Getletter(int page, int line, int letter)
        {
            if (line >= Pages[page].Lines.Length) return string.Empty;

            string[] letters = Pages[page].Lines[line].Split(' ');
            if (Pages.Length <= page ||
                Pages[page].Lines.Length <= line ||
                letters.Length <= letter)
                return string.Empty;
            return letters[letter];
        }
    }
}