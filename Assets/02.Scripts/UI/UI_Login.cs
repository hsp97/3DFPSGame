using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 로그인 화면. 로그인 패널과 회원가입 패널을 바꾸고, 입력을 확인해 계정 저장소에 부탁한다.
public class UI_Login : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _registerPanel;

    [Header("로그인 패널")]
    [SerializeField] private TMP_InputField _idInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TextMeshProUGUI _loginMessageText;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _gotoRegisterButton;

    [Header("회원가입 패널")]
    [SerializeField] private TMP_InputField _registerIdInput;
    [SerializeField] private TMP_InputField _registerPasswordInput;
    [SerializeField] private TMP_InputField _passwordConfirmInput;
    [SerializeField] private TextMeshProUGUI _registerMessageText;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _gotoLoginButton;

    [Header("기타")]
    [SerializeField] private string _gameSceneName = "GameScene";  // 로그인하면 불러올 씬

    private readonly PlayerPrefsAccountRepository _accountRepository = new PlayerPrefsAccountRepository();

    private void Start()
    {
        _loginButton.onClick.AddListener(Login);
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _registerButton.onClick.AddListener(Register);
        _gotoLoginButton.onClick.AddListener(GotoLogin);
        GotoLogin();
    }

    // 로그인: 입력을 확인하고, 맞으면 게임 씬을 불러온다.
    private void Login()
    {
        string id = _idInput.text;
        string password = _passwordInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            _loginMessageText.text = "아이디와 비밀번호를 입력해 주세요.";
            return;
        }

        if (!_accountRepository.CheckPassword(id, password))
        {
            // 아이디가 없는지 비밀번호가 틀렸는지는 알려 주지 않는다.
            _loginMessageText.text = "아이디 또는 비밀번호가 맞지 않습니다.";
            return;
        }

        SceneManager.LoadScene(_gameSceneName);
    }

    private void GotoRegister()
    {
        _loginPanel.SetActive(false);
        _registerPanel.SetActive(true);
        _registerMessageText.text = "";
    }

    private void GotoLogin()
    {
        _registerPanel.SetActive(false);
        _loginPanel.SetActive(true);
        _loginMessageText.text = "";
    }

    // 회원가입: 입력을 확인하고, 문제가 없으면 계정을 저장한 뒤 로그인 패널로 돌아간다.
    private void Register()
    {
        string id = _registerIdInput.text;
        string password = _registerPasswordInput.text;
        string passwordConfirm = _passwordConfirmInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            _registerMessageText.text = "아이디와 비밀번호를 입력해 주세요.";
            return;
        }

        if (!AccountIdSpecification.IsValidId(id))
        {
            _registerMessageText.text = AccountIdSpecification.ErrorMessage;
            return;
        }

        if (!AccountPasswordSpecification.IsValidPassword(password))
        {
            _registerMessageText.text = AccountPasswordSpecification.ErrorMessage;
            return;
        }

        if (password != passwordConfirm)
        {
            _registerMessageText.text = "비밀번호 확인이 맞지 않습니다.";
            return;
        }

        if (_accountRepository.Exists(id))
        {
            _registerMessageText.text = "이미 있는 아이디입니다.";
            return;
        }

        _accountRepository.Register(id, password);
        GotoLogin();
        _idInput.text = id;
        _loginMessageText.text = "가입했습니다. 로그인해 주세요.";
    }
}
