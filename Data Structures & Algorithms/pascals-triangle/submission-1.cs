public class Solution {
    public List<List<int>> Generate(int numRows) {
        var res = new List<List<int>> ();
        if(numRows <= 0) return res;

        res.Add(new List<int> { 1 });

        for (int i = 1; i < numRows; i++){
            var prev = res [i - 1];
            var temp = new List<int>(prev);
            temp.Insert(0, 0); // add 0 at start.
            temp.Add(0); //add 0 at end

            var row = new List<int>();
            //because each new row has exactly prev.Count + 1 elements.
            for (int j = 0; j < prev.Count + 1; j++) 
            {
                row.Add(temp[j] + temp[j + 1]);
            }
            res.Add(row);
        }
        return res;
    }
}