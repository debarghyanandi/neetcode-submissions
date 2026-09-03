public class Solution {
    public int MinEatingSpeed(int[] piles, int h) {
        // l and r is the range of speed
        int l = 1; // speed cant be 0;
        int r = 1;
        foreach (int pile in piles){
            // (highest size takes lowest time.)
            r = Math.Max(r, pile); // r is the highest size.
        }
        
        while( l < r )
        {
            int mid = l + (r-l)/2;
            //calcualte time required for thas mid value
            if(CanFinish(piles, mid, h)){
                r = mid;
            }
            else {
                l = mid + 1;
            }
        }
        return l;
        
    }
    private bool CanFinish(int[] piles, int speed, int targetHour){
        int hour = 0;
        foreach(int pile in piles){
            hour += (int)Math.Ceiling((double)pile / speed);
        //  hour += (pile + speed - 1) / speed; 
        }
        return hour <= targetHour;
    }
}
