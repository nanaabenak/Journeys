// using UnityEngine;

// public class OwareAI 
// {
//     public int GetMove(int[] playerPits, int[] aiPits)
//     {
//         int bestMove = -1;
//         int bestScore = int.MinValue;

//         for (int i =0; i <6 ; i++)
//         {
//             if (aiPits[i] == 0 ) continue;
            
//             int score = EvaluateMove(i,playerPits,aiPits);

//             if (score > bestScore)
//             {
//                 bestScore = score;
//                 bestMove = i;
//             }
//         }

//         if (Random.value < 0.3f)
//         {
//             return GetRandomMove(aiPits[bestMove]);
//         }
//         return bestMove;
//     }

//     int EvaluateMove(int pitIndex, int[] playerPits, int[] aiPits)
//     {
//         for (int i = 0; i < 6; i++)
//         {
//             if (aiPits[i] == 3)
//             {
//                 // i want to have it such that if there is any pit with three it should prioritize making that 4
//                 int stepsAway = pitIndex - i;
//                 if (aiPits[pitIndex] >= stepsAway)
//                 {
//                     score +=4;
//                 }
//                 else
//                 {
//                     score -=2;
//                 }
//             }
//         }

//         // prioritize capturing
//         if (pitIndex + aiPits[pitIndex] >= 6)
//         {
//             score +=2;
//         }
//         return score;
//     }

//     int GetRandomMove(int[] aiPits)
//     {
//         int index;
//         do
//         {
//             index = GetRandomMove.Range(0,6);
            
//         }while(aiPits[index] == 0);

//         return index;
//     }
// }
