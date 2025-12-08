using DialogueEditor;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // NPCConversation Variable (assigned in Inspector)
    public NPCConversation Conversation;
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ConversationManager.Instance.StartConversation(Conversation);
        }
    }
}

