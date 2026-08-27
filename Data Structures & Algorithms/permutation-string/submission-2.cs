public class Solution {
    public bool CheckInclusion(string s1, string s2) {
        if (s1.Length > s2.Length) return false;

        int[] need = new int[26];
        int[] window = new int[26];
        int matches = 0; // how many of the 26 letters currently have need[c] == window[c]

        for (int i = 0; i < s1.Length; i++) {
            need[s1[i] - 'a']++;
        }

        // A letter with need[c] == 0 already "matches" window[c] == 0 before anything is added.
        for (int c = 0; c < 26; c++) {
            if (need[c] == window[c]) matches++;
        }

        void Add(char ch) {
            int c = ch - 'a';
            if (window[c] == need[c]) matches--;   // about to break equality (if it was equal)
            window[c]++;
            if (window[c] == need[c]) matches++;   // may have restored equality
        }

        void Remove(char ch) {
            int c = ch - 'a';
            if (window[c] == need[c]) matches--;
            window[c]--;
            if (window[c] == need[c]) matches++;
        }

        for (int i = 0; i < s1.Length; i++) Add(s2[i]);
        if (matches == 26) return true;

        int left = 0;
        for (int right = s1.Length; right < s2.Length; right++) {
            Add(s2[right]);
            Remove(s2[left]);
            left++;
            if (matches == 26) return true;
        }

        return false;
    }
}