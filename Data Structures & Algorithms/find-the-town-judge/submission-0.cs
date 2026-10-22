public class Solution {
    public int FindJudge(int n, int[][] trust) {
        int [] outd = new int [n + 1];
        int [] ind = new int [n + 1];

        foreach (var item in trust)
        {
            int ai = item[0];
            int bi = item[1];
            ind[bi]++;
            outd[ai]++;
        }
        for (int i = 0; i <= n; i++){
            if(outd[i] == 0 && ind[i] == n - 1){
                return i;
            }
        }
        return -1;
    }
}