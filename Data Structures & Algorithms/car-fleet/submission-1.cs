public class Solution {
    public int CarFleet(int target, int[] position, int[] speed) {
        
        int n = position.Length;
        double [][] map = new double [n][];

        for ( int i = 0; i < n; i++ )
        {
            map[i] = new double[2];
            
            double time = (double) (target - position[i]) / speed [i];
            map[i][0] = position[i];
            map[i][1] = time;
        }

        Array.Sort(map, (a, b) => b[0].CompareTo(a[0]));

        int fleet = 0;
        double prevTime = 0;

        foreach (double[] car in map){
            if (car[1] > prevTime){
                fleet++;
                prevTime = car[1];
            }
        }
        return fleet;
    }
}
