using UnityEngine;


namespace TMPro.Examples
{
    public class TMP_TextEventCheck : MonoBehaviour
    {

        public TMP_TextEventHandler TextEventHandler;

        private TMP_Text m_TextComponent;

        void OnEnable()
        {
            if (TextEventHandler != null)
            {
                // Get a reference to the text component
                m_TextComponent = TextEventHandler.GetComponent<TMP_Text>();
                
                TextEventHandler.onCharacterSelection.AddListener(OnCharacterSelection);
                TextEventHandler.onSpriteSelection.AddListener(OnSpriteSelection);
                TextEventHandler.onWordSelection.AddListener(OnWordSelection);
                TextEventHandler.onLineSelection.AddListener(OnLineSelection);
                TextEventHandler.onLinkSelection.AddListener(OnLinkSelection);
            }
        }


        void OnDisable()
        {
            if (TextEventHandler != null)
            {
                TextEventHandler.onCharacterSelection.RemoveListener(OnCharacterSelection);
                TextEventHandler.onSpriteSelection.RemoveListener(OnSpriteSelection);
                TextEventHandler.onWordSelection.RemoveListener(OnWordSelection);
                TextEventHandler.onLineSelection.RemoveListener(OnLineSelection);
                TextEventHandler.onLinkSelection.RemoveListener(OnLinkSelection);
            }
        }


        void OnCharacterSelection(char c, int index)
        {
#if UNITY_EDITOR
            Debug.Log("Character [" + c + "] at Index: " + index + " has been selected.");
#endif
        }

        void OnSpriteSelection(char c, int index)
        {
#if UNITY_EDITOR
            Debug.Log("Sprite [" + c + "] at Index: " + index + " has been selected.");
#endif
        }

        void OnWordSelection(string word, int firstCharacterIndex, int length)
        {
#if UNITY_EDITOR
            Debug.Log("Word [" + word + "] with first character index of " + firstCharacterIndex + " and length of " + length + " has been selected.");
#endif
        }

        void OnLineSelection(string lineText, int firstCharacterIndex, int length)
        {
#if UNITY_EDITOR
            Debug.Log("Line [" + lineText + "] with first character index of " + firstCharacterIndex + " and length of " + length + " has been selected.");
#endif
        }

        void OnLinkSelection(string linkID, string linkText, int linkIndex)
        {
            if (m_TextComponent != null)
            {
                TMP_LinkInfo linkInfo = m_TextComponent.textInfo.linkInfo[linkIndex];
            }
            
#if UNITY_EDITOR
            Debug.Log("Link Index: " + linkIndex + " with ID [" + linkID + "] and Text \"" + linkText + "\" has been selected.");
#endif
        }

    }
}
