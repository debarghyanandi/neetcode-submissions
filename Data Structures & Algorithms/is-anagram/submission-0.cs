public class Solution {
    /*public bool IsAnagram(string s, string t) {

    //Length have to be equal
    if (s.Length != t.Length)
        return false;

    //Build a DicT of first String 
    Dictionary<char,int> anagram = new Dictionary<char,int>();
    foreach (char c in s)
    {
        if(anagram.ContainsKey(c))
        anagram[c]++;
        else
        anagram[c]=1;
    }

    //Now walk thru the same Dict and see the freq is same or not.
    foreach (char c in t){
        if(!anagram.ContainsKey(c))
        return false;

        anagram[c]--;

        //If count of any value is negative means that key is extra or diff.

        if (anagram[c]<0)
        return false;
    }
    return true;
    
    }
    Above solution is good, but as we only work with lowercase chars we can optimize*/
    
    public bool IsAnagram(string s, string t) {
    if (s.Length != t.Length)
        return false;

    int[] counts = new int[26];
    for (int i = 0; i < s.Length; i++)
    {
        counts[s[i] - 'a']++;
        counts[t[i] - 'a']--;
    }

    foreach (int c in counts)
        if (c != 0)
            return false;

    return true;
}
}
