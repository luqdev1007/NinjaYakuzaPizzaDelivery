using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class LeaderboardView : MonoBehaviour, IView
    {
        private void Start()
        {
            Debug.Log("Leaderboard feature: players can hide their profiles from viewing by donate value for N days");
            Debug.Log("Else show player in game profile: skin, perks but it has a more simplyfiers restrict by normal value and you need to 'steal' it from player");
            Debug.Log("You can steal player info if you outmatch your worst record with his best record in any map or on random if neither");
            Debug.Log("Or you can steal it by paying to other players who can outbest him insteat of yourself");
            Debug.Log("For more info check '#gamedev' tag in tg");
        }
    }
}