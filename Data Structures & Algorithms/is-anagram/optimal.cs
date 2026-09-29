// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Fixed-size array character balance   [array-balance-fixed]
// -  ties with optimal-variant.cs on O(n) time / O(1) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Fixed 26-slot array bounds space to constant; single pass processes
// -  both strings in O(n) time.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string s, string t)
    {
        if (s.Length != t.Length)
            return false;

        // One slot per lowercase letter. Index 0 = 'a', index 25 = 'z'.
        int[] letterBalance = new int[26];

        // Single pass over both strings at once: credit for s, debit for t.
        for (int i = 0; i < s.Length; i++)
        {
            letterBalance[s[i] - 'a']++;
            letterBalance[t[i] - 'a']--;
        }

        // Anagrams cancel out exactly, so every slot must be back to zero.
        foreach (int balance in letterBalance)
        {
            if (balance != 0)
                return false;
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Counting / Frequency Array - one balance per letter
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  letterBalance  letterBalance[c] = (count of letter c in s) - (count of letter c in t)
  balance        one slot of letterBalance, read in the final check
WHY THIS PATTERN
  An anagram means both strings use the same letters the same number of times,
  and the order does not matter. So we only need to count letters, not compare
  positions. The alphabet is small and fixed (26 lowercase letters), so a plain
  array works as the counter. letterBalance holds one net count per letter: s
  adds to it and t takes away from it.
BRUTE FORCE
  The first correct idea is to sort both strings and compare them. For example,
  turn each string into a char array, call Array.Sort, and check the two arrays
  with SequenceEqual. This takes O(n log n) time and O(n) extra space for the
  two copies. It loses because sorting puts the letters in order, and we never
  need that order. We only need the counts.
INVARIANT
  After step i of the loop, letterBalance[c] equals the count of letter c in
  s[0..i] minus its count in t[0..i]. When the loop ends, this covers both full
  strings. So every slot is zero exactly when every letter appears the same
  number of times in s and in t. That is the definition of an anagram.
LENGTH CHECK GUARDS THE SHARED INDEX
  The loop reads s[i] and t[i] with the same i, so it is only safe when the
  lengths are equal. The early return on s.Length != t.Length does two jobs. It
  skips work when the answer is clearly false. It also stops t[i] from going out
  of range, or stops the loop from ending before all of t is read.
WATCH OUT
  The code assumes every character is 'a' to 'z'. If the input has an uppercase
  letter, a digit, or a space, s[i] - 'a' gives an index outside 0..25. For
  example, 'A' - 'a' is -32, so the code throws IndexOutOfRangeException instead
  of returning false. If s or t is null, the code throws NullReferenceException
  on s.Length.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you return false early, before you read all of the input?
     Yes. Use two loops: first count up over s, then count down over t. Return
     false as soon as any slot goes below zero. This works because the lengths
     are equal, so if some letter has too many in t, another letter must go
     negative at some point. The cost is two passes instead of one combined
     pass.
  2. How do you group a list of words into anagram groups?
     Build a key for each word, such as its 26 counts joined into a string, or
     the word sorted. Use a Dictionary from key to List<string>. A counts key
     costs O(length) per word. A sorted key costs O(length log length).
  3. How do you find every substring of s that is an anagram of p?
     Use a sliding window the length of p with the same balance array. Add the
     letter that enters the window and remove the letter that leaves. Keep a
     count of slots that are not zero, so each step is O(1) and you do not
     rescan all 26 slots.
TRIGGER
  When a problem asks if two collections hold the same items the same number of
  times, and order does not matter, count them with a frequency array or map.
C# NOTE
  In C#, new int[26] always starts with every element set to 0, so letterBalance
  needs no setup loop. Note that s[i] - 'a' works on UTF-16 char values and
  gives back an int, so its result can be used directly as an array index.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
