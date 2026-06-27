using System;
using System.Text;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            input = input.ToLower();

            StringBuilder builder = new StringBuilder();

            foreach (char c in input)
            {
                if (!char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                {
                    builder.Append(c);
                }
            }
            string cleaned = builder.ToString();

            if (cleaned.Length == 0)
            {
                return false;
            }
            
            char[] charArray = cleaned.ToCharArray();
            Array.Reverse(charArray);
            string reversed = new string(charArray);

            return cleaned == reversed;
        }
    }
}
