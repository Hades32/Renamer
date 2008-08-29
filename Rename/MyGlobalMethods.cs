using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Rename
{
    public static class MyGlobalMethods
    {
        public static string ReplaceFirst(string s, string toReplace, string newVal)
        {
            var start = s.IndexOf(toReplace);
            if (start == 0)
                return newVal + s.Substring(toReplace.Length);
            else
                return s.Substring(0, start) + newVal + s.Substring(start + newVal.Length - 1);
        }

        public static bool Contains(IEnumerable e, object obj)
        {
            foreach (var o in e)
            {
                if (obj.Equals(o))
                    return true;
            }
            return false;
        }
    }
}
