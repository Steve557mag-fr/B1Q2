using UnityEngine;
using UnityEngine.Playables;

public class Minigame : MonoBehaviour
{
	internal bool isEnabled;
	internal bool isTimerLocked;
	internal float timeLeft, timeMax = 30;
	public ObjectsAnimatorController controllerGameplay, controllerDecoration;
	public PlayableDirector directorWin, directorLoose;
	bool isAlreadySetup = false;

    #region Directors

    private void OnLooseDirectorStopped(PlayableDirector obj)
    {
		DirectorUtils.CompleteStop(obj);
		if(GameManager.Get()) GameManager.Get().VerifySession();
    }

    private void OnWinDirectorStopped(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);
        if (GameManager.Get()) GameManager.Get().NextMG();
    }

    #endregion

    #region GameMethods

    internal void GameReset()
    {
		controllerDecoration.ForceResetAll();
		controllerGameplay.ForceResetAll();
    }

    internal void GameSetup()
	{
		isTimerLocked = true;
		timeLeft = timeMax;
		print("[MG]: GameSetup");

		if (!isAlreadySetup)
		{
			isAlreadySetup = true;
			directorWin.stopped += OnWinDirectorStopped;
			directorLoose.stopped += OnLooseDirectorStopped;
		}

		Setup();
        controllerDecoration.Play(true, () => { controllerGameplay.Play(true, GameBegin); });

    }

	internal void GameBegin()
	{
        isTimerLocked = false;
        isEnabled = true;
		Begin();
	}

    internal void GameTick()
	{
		if (isEnabled && !isTimerLocked) timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0, timeMax);
		else return;

		if (timeLeft <= 0) GameOver();
		else Tick();
    }

    internal void GameOver()
	{
		isEnabled = false;
		isTimerLocked  = true;
		print("[MG]: GameOver");
		Over();

        controllerGameplay.Play(false, () =>
		{
			directorLoose.Play();
		}, true);
	}

    internal void GameWin()
	{
        isEnabled = false;
        isTimerLocked = true;
		print("[MG]: GameWin");
        Win();
        
		controllerGameplay.Play(false, () =>
		{
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
