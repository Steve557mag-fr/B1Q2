using UnityEngine;
using UnityEngine.Playables;

public class Minigame : MonoBehaviour
{
	internal bool isEnabled;
	internal bool isLocked;
	internal float timeLeft, timeMax;
	public ObjectsAnimatorController Gameplay, Decoration;
	public PlayableDirector directorWin, directorLoose;
	bool isAlreadySetup = false;

    #region Directors

    private void OnLooseDirectorStopped(PlayableDirector obj)
    {
		DirectorUtils.CompleteStop(obj);
		GameManager.Get().VerifySession();
    }

    private void OnWinDirectorStopped(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);
        GameManager.Get().NextMG();
    }

    #endregion

    #region GameMethods

    internal void GameReset()
    {
        
    }

    internal void GameSetup()
	{
		timeLeft = timeMax;
		Gameplay.Play(true);
        Decoration.Play(true, GameBegin);
		Setup();

		if (isAlreadySetup) return;
		isAlreadySetup = true;
		directorWin.stopped += OnWinDirectorStopped;
		directorLoose.stopped += OnLooseDirectorStopped;
    }

	internal void GameBegin()
	{
		isEnabled = true;
		isLocked = true;
		Begin();
	}

    internal void GameTick()
	{
		if (isEnabled) timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0, timeMax);
		else return;

		if (timeLeft <= 0) GameOver();
		else Tick();
    }

    internal void GameOver()
	{
		isEnabled = false;
		isLocked  = false;
		Gameplay.Play(false, () =>
		{
			Over();
			directorLoose.Play();
		});
	}

    internal void GameWin()
	{
        isEnabled = false;
        isLocked = false;
		Gameplay.Play(false, () =>
		{
            Win();
            directorWin.Play();
        });
    }

    #endregion

    #region Overrides

    internal virtual void Setup()	{ }
    internal virtual void Begin()	{ }
    internal virtual void Tick()	{ }
    internal virtual void Over()	{ }
	internal virtual void Win()		{ }
	
	#endregion

}
