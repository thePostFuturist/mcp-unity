# MCP Unity Editor（ゲームエンジン）

[![](https://badge.mcpx.dev?status=on 'MCP 有効')](https://modelcontextprotocol.io/introduction)
[![](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=white 'Unity')](https://unity.com/releases/editor/archive)
[![](https://img.shields.io/badge/Node.js-339933?style=flat&logo=nodedotjs&logoColor=white 'Node.js')](https://nodejs.org/en/download/)
[![](https://img.shields.io/github/stars/CoderGamester/mcp-unity 'スター')](https://github.com/CoderGamester/mcp-unity/stargazers)
[![](https://img.shields.io/github/last-commit/CoderGamester/mcp-unity '最終コミット')](https://github.com/CoderGamester/mcp-unity/commits/main)
[![](https://img.shields.io/badge/License-MIT-red.svg 'MIT ライセンス')](https://opensource.org/licenses/MIT)

| [英語](README.md) | [🇨🇳簡体中文](README_zh-CN.md) | [🇯🇵日本語](README-ja.md) |
|----------------------|---------------------------------|----------------------|

```                                                                        
                              ,/(/.   *(/,                                  
                          */(((((/.   *((((((*.                             
                     .*((((((((((/.   *((((((((((/.                         
                 ./((((((((((((((/    *((((((((((((((/,                     
             ,/(((((((((((((/*.           */(((((((((((((/*.                
            ,%%#((/((((((*                    ,/(((((/(#&@@(                
            ,%%##%%##((((((/*.             ,/((((/(#&@@@@@@(                
            ,%%######%%##((/(((/*.    .*/(((//(%@@@@@@@@@@@(                
            ,%%####%#(%%#%%##((/((((((((//#&@@@@@@&@@@@@@@@(                
            ,%%####%(    /#%#%%%##(//(#@@@@@@@%,   #@@@@@@@(                
            ,%%####%(        *#%###%@@@@@@(        #@@@@@@@(                
            ,%%####%(           #%#%@@@@,          #@@@@@@@(                
            ,%%##%%%(           #%#%@@@@,          #@@@@@@@(                
            ,%%%#*              #%#%@@@@,             *%@@@(                
            .,      ,/##*.      #%#%@@@@,     ./&@#*      *`                
                ,/#%#####%%#/,  #%#%@@@@, ,/&@@@@@@@@@&\.                    
                 `*#########%%%%###%@@@@@@@@@@@@@@@@@@&*´                   
                    `*%%###########%@@@@@@@@@@@@@@&*´                        
                        `*%%%######%@@@@@@@@@@&*´                            
                            `*#%%##%@@@@@&*´                                 
                               `*%#%@&*´                                     
                                                        
     ███╗   ███╗ ██████╗██████╗         ██╗   ██╗███╗   ██╗██╗████████╗██╗   ██╗
     ████╗ ████║██╔════╝██╔══██╗        ██║   ██║████╗  ██║██║╚══██╔══╝╚██╗ ██╔╝
     ██╔████╔██║██║     ██████╔╝        ██║   ██║██╔██╗ ██║██║   ██║    ╚████╔╝ 
     ██║╚██╔╝██║██║     ██╔═══╝         ██║   ██║██║╚██╗██║██║   ██║     ╚██╔╝  
     ██║ ╚═╝ ██║╚██████╗██║             ╚██████╔╝██║ ╚████║██║   ██║      ██║   
     ╚═╝     ╚═╝ ╚═════╝╚═╝              ╚═════╝ ╚═╝  ╚═══╝╚═╝   ╚═╝      ╚═╝   
```       

MCP Unityは、Unityエディター向けのModel Context Protocolの実装であり、AIアシスタントがUnityプロジェクトと対話できるようにします。このパッケージは、UnityとMCPプロトコルを実装するNode.jsサーバー間のブリッジを提供し、Claude、Windsurf、CursorなどのAIエージェントがUnityエディター内で操作を実行できるようにします。

<a href="https://glama.ai/mcp/servers/@CoderGamester/mcp-unity">
  <img width="400" height="200" src="https://glama.ai/mcp/servers/@CoderGamester/mcp-unity/badge" alt="Unity MCPサーバー" />
</a>

## 機能

### IDE統合 - パッケージキャッシュアクセス

MCP Unityは、Unityの`Library/PackedCache`フォルダーをワークスペースに追加することで、VSCode系IDE（Visual Studio Code、Cursor、Windsurf）との自動統合を提供します。この機能により：

- Unityパッケージのコードインテリジェンスが向上
- Unityパッケージのより良いオートコンプリートと型情報が有効化
- AIコーディングアシスタントがプロジェクトの依存関係を理解するのに役立つ

### MCPサーバーツール

- `execute_menu_item`: Unityメニュー項目（MenuItem属性でタグ付けされた関数）を実行
  > **例:** "新しい空のGameObjectを作成するためにメニュー項目'GameObject/Create Empty'を実行"

- `select_gameobject`: パスまたはインスタンスIDでUnity階層内のゲームオブジェクトを選択
  > **例:** "シーン内のMain Cameraオブジェクトを選択"

- `update_gameobject`: GameObject のコアプロパティ（名前、タグ、レイヤー、アクティブ/静的状態）を更新、または存在しない場合は作成します
  > **例:** "Playerオブジェクトのタグを ‘Enemy’ に設定し、非アクティブにする"

- `update_component`: GameObject上のコンポーネントフィールドを更新、またはGameObjectに含まれていない場合は追加
  > **例:** "PlayerオブジェクトにRigidbodyコンポーネントを追加し、その質量を5に設定"

- `add_package`: Unityパッケージマネージャーに新しいパッケージをインストール
  > **例:** "プロジェクトにTextMeshProパッケージを追加"

- `run_tests`: Unityテストランナーを使用してテストを実行
  > **例:** "プロジェクト内のすべてのEditModeテストを実行"

- `send_console_log`: Unityにコンソールログを送信
  > **例:** "Unity Editorにコンソールログを送信"

- `add_asset_to_scene`: AssetDatabaseからアセットをUnityシーンに追加
  > **例:** "プロジェクトからPlayerプレハブを現在のシーンに追加"

- `take_screenshot`: ゲームビューをスクリーンショットして `Assets/Screenshots` に保存
  > **例:** "ゲームビューのスクリーンショットを撮影"

- `create_text_asset`: Unityプロジェクトにテキストファイルを作成
  > **例:** "Assets/DocsにREADME.txtファイルを作成"

- `get_text_asset`: Unityプロジェクト内のテキストファイルの内容を取得
  > **例:** "Assets/Docs/README.txt の内容を表示"


### MCPサーバーリソース

- `unity://menu-items`: `execute_menu_item`ツールを容易にするために、Unityエディターで利用可能なすべてのメニュー項目のリストを取得
  > **例:** "GameObject作成に関連する利用可能なすべてのメニュー項目を表示"

- `unity://scenes-hierarchy`: 現在のUnityシーン階層内のすべてのゲームオブジェクトのリストを取得
  > **例:** "現在のシーン階層構造を表示"

- `unity://gameobject/{id}`: シーン階層内のインスタンスIDまたはオブジェクトパスで特定のGameObjectに関する詳細情報を取得
  > **例:** "Player GameObjectに関する詳細情報を取得"

- `unity://logs`: Unityコンソールからのすべてのログのリストを取得
  > **例:** "Unityコンソールからの最近のエラーメッセージを表示"

- `unity://packages`: Unityパッケージマネージャーからインストール済みおよび利用可能なパッケージ情報を取得
  > **例:** "プロジェクトに現在インストールされているすべてのパッケージをリスト"

- `unity://assets`: Unityアセットデータベース内のアセット情報を取得
  > **例:** "プロジェクト内のすべてのテクスチャアセットを検索"

- `unity://tests/{testMode}`: Unityテストランナー内のテスト情報を取得
  > **例:** "プロジェクトで利用可能なすべてのテストをリスト"

## 要件
- Unity 2022.3以降 - [サーバーをインストール](#install-server)するため
- Node.js 18以降 - [サーバーを起動](#start-server)するため
- npm 9以降 - [サーバーをデバッグ](#debug-server)するため

## <a name="install-server"></a>インストール

このMCP Unityサーバーのインストールは複数ステップのプロセスです：

### ステップ1: Unityパッケージマネージャー経由でUnity MCPサーバーパッケージをインストール
1. Unityパッケージマネージャーを開く（Window > Package Manager）
2. 左上隅の"+"ボタンをクリック
3. "Add package from git URL..."を選択
4. 入力: `https://github.com/CoderGamester/mcp-unity.git`
5. "Add"をクリック

![package manager](https://github.com/user-attachments/assets/a72bfca4-ae52-48e7-a876-e99c701b0497)


### ステップ2: Node.jsをインストール
> MCP Unityサーバーを実行するには、コンピューターにNode.js 18以降がインストールされている必要があります：

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Windows</span></summary>

1. [Node.jsダウンロードページ](https://nodejs.org/en/download/)にアクセス
2. LTSバージョンのWindowsインストーラー（.msi）をダウンロード（推奨）
3. インストーラーを実行し、インストールウィザードに従う
4. PowerShellを開いて以下を実行してインストールを確認：
   ```bash
   node --version
   ```
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">macOS</span></summary>

1. [Node.jsダウンロードページ](https://nodejs.org/en/download/)にアクセス
2. LTSバージョンのmacOSインストーラー（.pkg）をダウンロード（推奨）
3. インストーラーを実行し、インストールウィザードに従う
4. または、Homebrewがインストールされている場合は以下を実行：
   ```bash
   brew install node@18
   ```
5. ターミナルを開いて以下を実行してインストールを確認：
   ```bash
   node --version
   ```
</details>

### ステップ3: AI LLMクライアントを設定

<details open>
<summary><span style="font-size: 1.1em; font-weight: bold;">オプション1: Unityエディターを使用して設定</span></summary>

1. Unityエディターを開く
2. Tools > MCP Unity > Server Windowに移動
3. 以下の画像のようにAI LLMクライアントの"Configure"ボタンをクリック

![image](https://github.com/user-attachments/assets/8d286e83-da60-40fa-bd6c-5de9a77c1820)

4. 表示されるポップアップで設定インストールを確認

![image](https://github.com/user-attachments/assets/b1f05d33-3694-4256-a57b-8556005021ba)

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">オプション2: 手動設定</span></summary>

AIクライアントのMCP設定ファイル（例：Claude Desktopのclaude_desktop_config.json）を開き、以下のテキストをコピー：

> `ABSOLUTE/PATH/TO`をMCP Unityインストールの絶対パスに置き換えるか、UnityエディターMCPサーバーウィンドウ（Tools > MCP Unity > Server Window）からテキストをコピー

```json
{
  "mcpServers": {
    "mcp-unity": {
      "command": "node",
      "args": [
        "ABSOLUTE/PATH/TO/mcp-unity/Server~/build/index.js"
      ]
    }
  }
}
```

</details>

## <a name="start-server"></a>サーバーの起動

MCP Unityサーバーを起動するには2つの方法があります：

## オプション: Node.jsサーバーのインストール
デフォルトでは、Node.jsサーバーは `Server~/` ディレクトリにインストールされます。
問題が発生した場合は、以下の手順で強制的にインストールできます：

1. Unityエディターを開く
2. メニューから Tools > MCP Unity > Server Window に移動
3. 「Force Install Server」ボタンをクリック

> [!TIP]
> Node.js サーバーは `Server~/` ディレクトリにインストールされます。


### オプション1: Unityエディター経由で起動
1. Unityエディターを開く
2. Tools > MCP Unity > Server Windowに移動
3. "Start Server"ボタンをクリック

### オプション2: コマンドラインから起動
1. ターミナルまたはコマンドプロンプトを開く
2. MCP Unityサーバーディレクトリに移動
3. 以下のコマンドを実行：
   ```bash
   node Server~/build/index.js
   ```

## オプション: タイムアウト設定

デフォルトでは、MCPサーバーとWebSocket間のタイムアウトは 10 秒です。
お使いの環境に応じて以下の手順で変更できます：

1. Unityエディターを開く  
2. **Tools > MCP Unity > Server Window** に移動  
3. **Request Timeout (seconds)** の値を希望のタイムアウト秒数に変更  
4. Unityが環境変数 `UNITY_REQUEST_TIMEOUT` に新しい値を設定  
5. Node.jsサーバーを再起動  
6. **Start Server** を再度クリックして再接続  

> [!TIP]  
> AIコーディングIDE（Claude Desktop、Cursor IDE、Windsurf IDEなど）とMCPサーバー間のタイムアウトは、使用するIDEによって異なる場合があります。

## サポート & フィードバック

ご質問やサポートが必要な場合は、このリポジトリの[Issue](https://github.com/CoderGamester/mcp-unity/issues)を開くか、以下までご連絡ください：
- LinkedIn: [![](https://img.shields.io/badge/LinkedIn-0077B5?style=flat&logo=linkedin&logoColor=white 'LinkedIn')](https://www.linkedin.com/in/miguel-tomas/)
- Discord: gamester7178
- Email: game.gamester@gmail.com

## 貢献

貢献は大歓迎です！プルリクエストを送信するか、Issueを開いてください。

**コミットメッセージ**は[Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/)形式に従ってください。

## ライセンス

本プロジェクトは[MIT License](License.md)の下で提供されます。

## 謝辞

- Model Context Protocol
- Unity Technologies
- Node.js
- WebSocket-Sharp
