using Unity.VisualScripting;
using UnityEngine;

namespace DFramework
{
    /*
     * CLASS: SingletonMonoBehavior
     * 
     * DESCRIPTION:
     * This class creates a singleton object that will persist through all game scenes.
     * 
     * IMPORTANT:
     * Singleton behavior is implemented immediately upon inheritance through the Awake function.
     * If Awake is overriden, make sure to call to the base.Awake implementation to maintain 
     * Singleton functionality.
     */
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour
        where T : SingletonMonoBehavior<T>
    {
        static private T _instance;

        public T Instance { get => _instance; }

        protected virtual void Awake()
        {
            // We've already created a singleton instance, so destroy this game object.
            if (_instance != null)
            {
                Destroy(this);
                return;
            }

            // first instance of this game object in the game scene.
            // Save this so we can access the data again during game over scene.
            _instance = this as T;
            DontDestroyOnLoad(this);
        }
    }
}
