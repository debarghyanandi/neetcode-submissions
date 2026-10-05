// Practice
public class Solution {
    public int MinEatingSpeed(int[] piles, int h) {
        int l = 1;
        int r = 0;
        
        foreach(int pile in piles){
            r = Math.Max(r, pile);
        }

        while (l < r)
        {
            int mid = l + (r - l) / 2;

            if(CanFinish(piles, mid, h))
            {
                r = mid;
            }
            else
                l = mid + 1;
        }
        return r;
    }

    private bool CanFinish (int [] piles, int speed, int h){
        int hour = 0;
        foreach(int pile in piles){
            hour += (int)Math.Ceiling((double)pile / speed);
        }
        return hour <= h;
    }
}
