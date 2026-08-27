public class Solution {
    public bool CheckInclusion(string s1, string s2) {
        
        if (s1.Length > s2.Length)
        return false;
        
        var counts1 = new Dictionary<char, int>();
        var counts2 = new Dictionary<char, int>();
        
        for (int i = 0; i < s1.Length; i++){
            if(counts1.ContainsKey(s1[i])){
                counts1[s1[i]]++;
            }
            else counts1.Add(s1[i], 1);
            
            if(counts2.ContainsKey(s2[i])){
                counts2[s2[i]]++;
            }
            else counts2.Add(s2[i], 1);
        }
        

        int left = 0;
        int right = s1.Length - 1;
        while (right < s2.Length){
            
            if(counts1.Count == counts2.Count &&
                    counts1.All(pair =>
                    counts2.TryGetValue(pair.Key, out int value) &&
                    value == pair.Value))
                return true;
            
            else
            {
                if(counts2[s2[left]] > 1)
                    counts2[s2[left]]--;
                else counts2.Remove(s2[left]);
                left++;
                right++;
                if(right==s2.Length)
                return false;
                if(counts2.ContainsKey(s2[right]))
                {
                    counts2[s2[right]]++;
                }
                else counts2.Add(s2[right], 1);
            }
        }
        return false;

    }
}
