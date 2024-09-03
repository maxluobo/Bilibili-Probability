using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RC.Main;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private ReferenceCollector _rc;
    private InputField _csInputField;
    private InputField _ohInputField;
    private InputField _ycInputField;
    private InputField _priceInputField;
    private Text _totalPrice;
    private Text _totalTimes;
    private Image _resultItem;
    private Button _oneBtn;
    private Button _tenBtn;
    private Button _clearBtn;
    private Button _yetBtn;
    private Text _csTimes;
    private Text _ohTimes;
    private Text _ycTimes;
    private GameObject _tipDialog;
    private Image _tipImage;
    private Text _tipText;
    private Button _tipBtn;
    private Button _expectBtn;
    private GameObject _expectDialog;
    private Text _expectText;

    private List<Image> _handleList = new List<Image>();
    public Sprite[] sprites;
    private int _totalTimesValue;
    private int _totalPriceValue;
    private int _lastPriceValue;
    private int _csTimesValue;
    private int _ohTimesValue;
    private int _ycTimesValue;

    private enum Type
    {
        Cs = 0,
        Oh,
        Yc,
        Lj
    }

    private void Awake()
    {
        //设置相关
        Screen.orientation = ScreenOrientation.Portrait;
        Application.targetFrameRate = 120;

        _rc = GetComponent<ReferenceCollector>();
        _csInputField = _rc.GetComponent<InputField>("CSInput");
        _ohInputField = _rc.GetComponent<InputField>("OHInput");
        _ycInputField = _rc.GetComponent<InputField>("YCInput");
        _priceInputField = _rc.GetComponent<InputField>("PriceInput");
        _totalPrice = _rc.GetComponent<Text>("TotalPrice");
        _totalTimes = _rc.GetComponent<Text>("TotalTimes");
        _resultItem = _rc.GetComponent<Image>("ResultItem");
        _oneBtn = _rc.GetComponent<Button>("OneBtn");
        _tenBtn = _rc.GetComponent<Button>("TenBtn");
        _clearBtn = _rc.GetComponent<Button>("ClearBtn");
        _yetBtn = _rc.GetComponent<Button>("YetBtn");
        _csTimes = _rc.GetComponent<Text>("CSTimes");
        _ohTimes = _rc.GetComponent<Text>("OHTimes");
        _ycTimes = _rc.GetComponent<Text>("YCTimes");
        _tipDialog = _rc.GetGameObject("TipDialog");
        _tipImage = _rc.GetComponent<Image>("TipImage");
        _tipText = _rc.GetComponent<Text>("TipText");
        _tipBtn = _rc.GetComponent<Button>("TipBtn");
        _expectBtn = _rc.GetComponent<Button>("ExpectBtn");
        _expectDialog = _rc.GetGameObject("ExpectDialog");
        _expectText = _rc.GetComponent<Text>("ExpectText");

        _oneBtn.onClick.AddListener(() => ChouKa());
        _tenBtn.onClick.AddListener(() =>
        {
            for (var i = 0; i < 10; i++) ChouKa();
        });
        _yetBtn.onClick.AddListener(ChouKaYet);
        _clearBtn.onClick.AddListener(Clear);
        _tipBtn.onClick.AddListener(() => _tipDialog.SetActive(false));
        _expectBtn.onClick.AddListener(Expect);
    }

    private int ChouKa()
    {
        var handelPro = new List<int>
        {
            (int)(GetInputValue(_csInputField) * 100),
            (int)(GetInputValue(_ohInputField) * 100),
            (int)(GetInputValue(_ycInputField) * 100),
        };
        var random = Random.Range(0, 10000);
        var result = 3;
        for (var i = 0; i < handelPro.Count; i++)
        {
            if (handelPro[i] == 0) continue;
            if (random <= handelPro[i])
            {
                result = i;
                break;
            }

            random -= handelPro[i];
        }

        _lastPriceValue += (int)GetInputValue(_priceInputField);
        switch ((Type)result)
        {
            case Type.Cs:
                _csTimes.text = (++_csTimesValue).ToString();
                break;
            case Type.Oh:
                _ohTimes.text = (++_ohTimesValue).ToString();
                break;
            case Type.Yc:
                _ycTimes.text = (++_ycTimesValue).ToString();
                break;
        }

        if (result != (int)Type.Lj)
        {
            _tipDialog.SetActive(true);
            _tipImage.sprite = sprites[result];
            _tipImage.transform.DOScale(0, 0.3f).From();
            _tipText.text = $"此款花费{_lastPriceValue}元";
            _lastPriceValue = 0;
        }

        var item = Instantiate(_resultItem, _resultItem.transform.parent);
        item.sprite = sprites[result];
        item.gameObject.SetActive(true);
        _handleList.Add(item);
        _totalTimes.text = (++_totalTimesValue).ToString();
        _totalPrice.text = (_totalPriceValue += (int)GetInputValue(_priceInputField)).ToString();
        return result;
    }

    private void ChouKaYet()
    {
        if (GetInputValue(_csInputField) + GetInputValue(_ohInputField) + GetInputValue(_ycInputField) == 0)
        {
            return;
        }

        while (true)
        {
            var result = ChouKa();
            if (result != (int)Type.Lj) break;
        }
    }

    private void Expect()
    {
        Clear();
        var btn = _expectDialog.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        _expectDialog.SetActive(true);
        _expectText.text = "计算中...";
        for (var j = 0; j < 100000; j++)
        {
            var handelPro = new List<int>
            {
                (int)(GetInputValue(_csInputField) * 100),
                (int)(GetInputValue(_ohInputField) * 100),
                (int)(GetInputValue(_ycInputField) * 100),
            };
            var random = Random.Range(0, 10000);
            var result = 3;
            for (var i = 0; i < handelPro.Count; i++)
            {
                if (handelPro[i] == 0) continue;
                if (random <= handelPro[i])
                {
                    result = i;
                    break;
                }

                random -= handelPro[i];
            }

            _lastPriceValue += (int)GetInputValue(_priceInputField);
            switch ((Type)result)
            {
                case Type.Cs:
                    _csTimes.text = (++_csTimesValue).ToString();
                    break;
                case Type.Oh:
                    _ohTimes.text = (++_ohTimesValue).ToString();
                    break;
                case Type.Yc:
                    _ycTimes.text = (++_ycTimesValue).ToString();
                    break;
            }
        }

        var allPrice = (int)GetInputValue(_priceInputField) * 100000;
        _expectText.text = $"执行10W次结果如下:\r\n";
        if (_csTimesValue == 0)
            _expectText.text += $"未出现超神款\r\n";
        else
            _expectText.text += $"超神款花费期望：{allPrice / (float)_csTimesValue}\r\n";
        if (_ohTimesValue == 0)
            _expectText.text += $"未出现欧皇款\r\n";
        else
            _expectText.text += $"欧皇款花费期望：{allPrice / (float)_ohTimesValue}\r\n";
        if (_ycTimesValue == 0)
            _expectText.text += $"未出现隐藏款\r\n";
        else
            _expectText.text += $"隐藏款花费期望：{allPrice / (float)_ycTimesValue}\r\n";
        if ((_csTimesValue + _ohTimesValue + _ycTimesValue) != 0)
            _expectText.text += $"总花费期望：{allPrice / (_csTimesValue + _ohTimesValue + _ycTimesValue)}";
        btn.onClick.AddListener(() => _expectDialog.SetActive(false));
    }

    private float GetInputValue(InputField inputField) =>
        string.IsNullOrEmpty(inputField.text) ? 0 : (float.Parse(inputField.text));

    private void Clear()
    {
        _totalTimes.text = "0";
        _totalPrice.text = "0";
        _csTimes.text = "0";
        _ohTimes.text = "0";
        _ycTimes.text = "0";
        _totalTimesValue = 0;
        _totalPriceValue = 0;
        _lastPriceValue = 0;
        _csTimesValue = 0;
        _ohTimesValue = 0;
        _ycTimesValue = 0;
        foreach (var o in _handleList)
        {
            Destroy(o.gameObject);
        }

        _handleList.Clear();
    }
}