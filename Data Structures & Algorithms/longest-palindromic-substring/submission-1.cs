public class Solution {
    public string LongestPalindrome(string s) {
        int start = 0;
        int maxLength = 0;

        for (int i = 0; i < s.Length; i++) {

            // Odd-length palindrome
            Expand(i, i);

            // Even-length palindrome
            Expand(i, i + 1);
        }

        return s.Substring(start, maxLength);

        void Expand(int left, int right) {
            while (left >= 0 && right < s.Length &&
                   s[left] == s[right]) {

                int length = right - left + 1;

                if (length > maxLength) {
                    maxLength = length;
                    start = left;
                }

                left--;
                right++;
            }
        }
    }
}