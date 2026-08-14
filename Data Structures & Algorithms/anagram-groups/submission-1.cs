public class Solution {
    public List<List<string>> GroupAnagrams(string[] strs) {
        //Check Solution
        var res = new Dictionary<string, List<string>>();
        
        foreach (var s in strs) {
            char[] charArray = s.ToCharArray();
            Array.Sort(charArray);
            string sortedS = new string(charArray);
            
            //Optimized lookup
            if (!res.TryGetValue(sortedS, out List<string> group)) {
                group = new List<string>();
                res[sortedS] = group;
            }
            group.Add(s);
        }
        return res.Values.ToList();
    }
}