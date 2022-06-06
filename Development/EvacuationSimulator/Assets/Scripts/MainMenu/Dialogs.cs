using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogs : MonoBehaviour
{
    public static GameObject NavigationDialog;
    public static GameObject FireDialog;
    public static GameObject BotsDialog;

    public static Action OnDialogClose { get; set; }

    private static TMPro.TextMeshProUGUI _navigationText;
    private static TMPro.TextMeshProUGUI _fireText;
    private static TMPro.TextMeshProUGUI _botsText;

    private static bool _showNavigationDIalog;
    private static bool _showFireDIalog;
    private static bool _showBotsDIalog;

    private static int _counter = 1;
    private static int Counter
    {
        get => _counter;
        set
        {
            if(value > 14)
            {
                _counter = 1;
                return;
            }
            _counter = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NavigationDialog = transform.GetChild(0).gameObject;
        FireDialog = transform.GetChild(1).gameObject;
        BotsDialog = transform.GetChild(2).gameObject;

        _navigationText = NavigationDialog.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        _fireText = FireDialog.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        _botsText = BotsDialog.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
    }

    public static IEnumerator ShowNavigationDialog()
    {
        _showNavigationDIalog = true;
        NavigationDialog.SetActive(true);
        while (_showNavigationDIalog)
        {
            _navigationText.text = new string('.', Counter++);
            yield return new WaitForSeconds(1);
        }
        NavigationDialog.SetActive(false);
        Counter = 1;
        OnDialogClose?.Invoke();
    } 

    public static void StopShowNavigationDialog()
    {
        _showNavigationDIalog = false;
    }

    public static IEnumerator ShowFireDialog()
    {
        _showFireDIalog = true;
        FireDialog.SetActive(true);
        while (_showFireDIalog)
        {
            _fireText.text = new string('.', Counter++);
            yield return new WaitForSeconds(1);
        }
        FireDialog.SetActive(false);
        Counter = 1;
        OnDialogClose?.Invoke();
    }

    public static void StopShowFireDialog()
    {
        _showFireDIalog = false;
    }

    public static IEnumerator ShowBotsDialog()
    {
        _showBotsDIalog = true;
        BotsDialog.SetActive(true);
        while (_showBotsDIalog)
        {
            _botsText.text = new string('.', Counter++);
            yield return new WaitForSeconds(1);
        }
        BotsDialog.SetActive(false);
        Counter = 1;
        OnDialogClose?.Invoke();
    }

    public static void StopShowBotsDialog()
    {
        _showBotsDIalog = false;
    }
}
