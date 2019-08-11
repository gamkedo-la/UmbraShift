
using FMOD.Studio;

public static class FMODExtensions
{


    /// <summary>
    /// Determines if this Event Instance is playing or stopped.
    
    /// </summary>
    /// <returns><c>true</c> if the event is playing <c>false</c>.</returns>
    public static bool IsPlaying (this EventInstance eventInstance)
    {

        PLAYBACK_STATE playbackState;
        eventInstance.getPlaybackState(out playbackState);
        bool isPlaying = playbackState != PLAYBACK_STATE.STOPPED;

        return isPlaying;

    }
}
