using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using EnigmaBreaker.Screens.Helpers;
using EnigmaBreaker.Model;
using System.Diagnostics;

namespace EnigmaBreaker.Manager
{
    /// <summary>
    /// The sound manager class manages all game sounds. 
    /// It maintains a list of all sound effects and background music.
    /// </summary>
    public class SoundManager : GameComponent
    {

        private static SoundManager soundManager = null;


        #region Audio Data


        private AudioEngine audioEngine;
        private SoundBank soundBank;
        private WaveBank waveBank;
        private WaveBank musicBank;
        private Cue musicCue;
        private float audioVolume;
        AudioCategory audioCategory;


        #endregion


        #region Initialization Methods


        /// <summary>
        /// Constructs the manager for audio playback of all cues.
        /// </summary>
        /// <param name="game">The game that this component will be attached to.</param>
        private SoundManager(Game game)
            : base(game)
        {
            try
            {
                audioEngine = new AudioEngine(@"Content\Audio\audio.xgs");
                waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
                musicBank = new WaveBank(audioEngine, @"Content\Audio\Music Bank.xwb", 0, 16);
                soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            }
            catch (NoAudioHardwareException)
            {
                // silently fall back to silence
                audioEngine = null;
                waveBank = null;
                musicBank = null;
                soundBank = null;
            }
        }


        /// <summary>
        /// Initialize the static SoundManager functionality.
        /// </summary>
        /// <param name="game">The game that this component will be attached to.</param>
        /// <param name="sc">The screen manager component, that will be used to update the volume.</param>
        public static void Initialize(Game game, ScreenManager sc)
        {
            soundManager = new SoundManager(game);
            if (game != null)
            {
                game.Components.Add(soundManager);
            }
            LoadVolume(sc);
            soundManager.audioCategory = soundManager.audioEngine.GetCategory("Default");
            UpdateVolume();
        }

        private static void LoadVolume(ScreenManager sc)
        {
            try
            {
                OptionData data = OptionsHelper.LoadOptions(OptionsHelper.OptionsFilename);

                soundManager.audioVolume = float.Parse((double.Parse(data.SoundVolume.ToString()) / 100).ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);

                soundManager.audioVolume = .3f;
            }
        }


        #endregion


        #region Cue Methods


        /// <summary>
        /// Retrieve a cue by name.
        /// </summary>
        /// <param name="cueName">The name of the cue requested.</param>
        /// <returns>The cue corresponding to the name provided.</returns>
        public static Cue GetCue(string cueName)
        {
            if ((soundManager == null) || (soundManager.audioEngine == null) ||
                (soundManager.soundBank == null) || (soundManager.waveBank == null) || (soundManager.musicBank == null))
            {
                return null;
            }
            return soundManager.soundBank.GetCue(cueName);
        }


        /// <summary>
        /// Plays a cue by name.
        /// </summary>
        /// <param name="cueName">The name of the cue to play.</param>
        public static void PlayCue(string cueName)
        {
            if ((soundManager != null) && (soundManager.audioEngine != null) &&
                (soundManager.soundBank != null) && (soundManager.waveBank != null) || (soundManager.musicBank == null))
            {
                soundManager.soundBank.PlayCue(cueName);
            }
        }


        /// <summary>
        /// Play music by cue name.
        /// </summary>
        /// <param name="musicCueName">The name of the music cue.</param>
        public static void PlayMusic(string musicCueName)
        {
            if ((soundManager == null) || (soundManager.audioEngine == null) ||
                (soundManager.soundBank == null) || (soundManager.waveBank == null) || (soundManager.musicBank == null))
            {
                return;
            }
            // stop the old music cue
            if (soundManager.musicCue != null)
            {
                soundManager.musicCue.Stop(AudioStopOptions.AsAuthored);
            }
            // get the new music cue, if any
            soundManager.musicCue = GetCue(musicCueName);
            // start the new music cue, if any
            if (soundManager.musicCue != null)
            {
                soundManager.musicCue.Play();
            }
        }

        public static void PauseMusic()
        {
            if (soundManager.musicCue != null)
            {
                soundManager.musicCue.Pause();
            }
        }

        public static void ResumePlay()
        {
            if (soundManager.musicCue != null && soundManager.musicCue.IsPaused)
            {
                soundManager.musicCue.Resume();
            }
        }

        public static void StopMusic()
        {
            if (soundManager.musicCue != null)
            {
                soundManager.musicCue.Stop(AudioStopOptions.Immediate);
            }
        }


        #endregion


        #region Updating Methods


        /// <summary>
        /// Update the audio manager, particularly the engine.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // update the audio engine
            if (audioEngine != null)
            {
                audioEngine.Update();
            }

            base.Update(gameTime);
        }


        #endregion


        #region Volume Controll

        public static void IncreaseVolume()
        {
            soundManager.audioVolume += 0.05f;

            if (soundManager.audioVolume > 1.0f)
            {
                soundManager.audioVolume = 1.0f;
            }
            UpdateVolume();
        }
        public static void DecreaseVolume()
        {
            soundManager.audioVolume -= 0.05f;

            if (soundManager.audioVolume < 0.0f)
            {
                soundManager.audioVolume = 0.0f;
            }
            UpdateVolume();
        }

        public static float Volume()
        {
            return soundManager.audioVolume;
        }

        private static void UpdateVolume()
        {
            soundManager.audioCategory.SetVolume(soundManager.audioVolume);
        }


        #endregion

        #region Instance Disposal Methods

        /// <summary>
        /// Clean up the component when it is disposing.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (soundBank != null)
                    {
                        soundBank.Dispose();
                        soundBank = null;
                    }
                    if (waveBank != null)
                    {
                        waveBank.Dispose();
                        waveBank = null;
                    }
                    if (musicBank != null)
                    {
                        musicBank.Dispose();
                        musicBank = null;
                    }
                    if (audioEngine != null)
                    {
                        audioEngine.Dispose();
                        audioEngine = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
