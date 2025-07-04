using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public CharacterStats PlayerStats;

    List<IEndGameObserver> m_EndGameObservers = new List<IEndGameObserver>();
    
    public void RegisterPlayer(CharacterStats player)
    {
        PlayerStats = player;
    }
    
    public void RegisterEndGameObserver(IEndGameObserver observer)
    {
        m_EndGameObservers.Add(observer);
    }

    public void RemoveEndGameObserver(IEndGameObserver observer)
    {
        m_EndGameObservers.Remove(observer);
    }

    public async void NotifyPlayerDeadAsync()
    {
        foreach (var observer in m_EndGameObservers)
        {
            observer.OnEndGame();
        }
    }
}