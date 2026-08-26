public class Solution
{
    public int CharacterReplacement(string s, int k)
    {
        // My solution
        int maxLength = 0;
        var uniqueCharacters = new HashSet<char>(s);

        foreach (char targetChar in uniqueCharacters)
        {
            int left = 0; 
            int count = 0;

            for (int right = 0; right < s.Length; right++)
            {
                if (s[right] == targetChar)
                {
                    count++;
                }

                while((right - left + 1) - count > k)
                {
                    if(s[left] == targetChar)
                    {
                        count--;
                    }
                    
                    left++;
                }

                maxLength = Math.Max(maxLength, right - left + 1);
            }
        }

        return maxLength;
    }

}