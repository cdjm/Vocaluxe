﻿using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace VocaluxeLib.Menu
{
    public class CXMLReader
    {
        private readonly XPathNavigator _Navigator;
        private readonly String _FileName;

        public string FileName
        {
            get { return _FileName; }
        }

        //Private method. Use OpenFile factory method to get an instance
        private CXMLReader(string uri)
        {
            _FileName = uri;
            XPathDocument xPathDoc = new XPathDocument(uri);
            _Navigator = xPathDoc.CreateNavigator();
        }

        public XPathNavigator Navigator
        {
            get { return _Navigator; }
        }

        public static CXMLReader OpenFile(string sFile)
        {
            try
            {
                return new CXMLReader(sFile);
            }
            catch (Exception e)
            {
                CBase.Log.LogError("Can't open XML file: " + sFile + ": " + e.Message);
                return null;
            }
        }

        public bool TryGetEnumValue<T>(string Cast, ref T value)
            where T : struct
        {
            string val = String.Empty;
            if (GetValue(Cast, ref val, Enum.GetName(typeof(T), value)))
            {
                CHelper.TryParse(val, out value, true);
                return true;
            }
            return false;
        }

        public bool TryGetIntValue(string Cast, ref int value)
        {
            string val = String.Empty;
            if (GetValue(Cast, ref val, value.ToString()))
                return int.TryParse(val, out value);
            return false;
        }

        public bool TryGetIntValueRange(string Cast, ref int value, int min = 0, int max = 100)
        {
            bool result = TryGetIntValue(Cast, ref value);
            if (result)
            {
                if (value < min)
                    value = min;
                else if (value > max)
                    value = max;
            }
            return result;
        }

        public bool TryGetFloatValue(string Cast, ref float value)
        {
            string val = String.Empty;
            if (GetValue(Cast, ref val, value.ToString()))
                return CHelper.TryParse(val, out value);
            return false;
        }

        public bool GetValue(string Cast, ref string Value, string DefaultValue)
        {
            XPathNodeIterator iterator;
            int results = 0;
            string val = string.Empty;

            _Navigator.MoveToFirstChild();
            iterator = _Navigator.Select(Cast);

            while (iterator.MoveNext())
            {
                val = iterator.Current.Value;
                results++;
            }

            if ((results == 0) || (results > 1))
            {
                Value = DefaultValue;
                return false;
            }
            else
            {
                Value = val;
                return true;
            }
        }

        public List<string> GetValues(string Cast)
        {
            List<string> values = new List<string>();

            _Navigator.MoveToRoot();
            _Navigator.MoveToFirstChild();
            _Navigator.MoveToFirstChild();

            while (_Navigator.Name != Cast)
                _Navigator.MoveToNext();

            _Navigator.MoveToFirstChild();

            values.Add(_Navigator.LocalName);
            while (_Navigator.MoveToNext())
                values.Add(_Navigator.LocalName);

            return values;
        }

        public List<string> GetAttributes(string Cast, string attribute)
        {
            List<string> values = new List<string>();

            _Navigator.MoveToRoot();
            _Navigator.MoveToFirstChild();

            while (_Navigator.Name != Cast)
                _Navigator.MoveToNext();

            _Navigator.MoveToFirstChild();

            values.Add(_Navigator.LocalName);
            while (_Navigator.MoveToNext())
                values.Add(_Navigator.GetAttribute(attribute, ""));

            return values;
        }

        public bool GetInnerValues(string Cast, ref List<string> Values)
        {
            _Navigator.MoveToRoot();
            _Navigator.MoveToFirstChild();
            _Navigator.MoveToFirstChild();

            while (_Navigator.Name != Cast)
                _Navigator.MoveToNext();

            _Navigator.MoveToFirstChild();

            Values.Add(_Navigator.Value);
            while (_Navigator.MoveToNext())
                Values.Add(_Navigator.Value);

            return true;
        }

        public bool ItemExists(string Cast)
        {
            XPathNodeIterator iterator;
            int results = 0;

            _Navigator.MoveToFirstChild();
            iterator = _Navigator.Select(Cast);

            while (iterator.MoveNext())
                results++;

            if (results == 0)
                return false;

            return true;
        }
    }
}