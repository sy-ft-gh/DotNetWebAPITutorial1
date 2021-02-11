using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using WebApplication1.Models;


namespace WebApplication1.Controllers {

    [RoutePrefix("Products")]
    public class ProductsController : ApiController {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };
        // URLとアクションのルーティングについて
        // 初期状態のルーティングでは
        // 「/api/Products」 に対応する
        // Getメソッドでの要求時には「Get～」「GetAl～」を
        //「/api/Products」で検索する。
        // 「/api/Products」の例ではGetProductsまたはGetAllProductsが
        // 該当する。
        // また、idがある場合、例えば「/api/Products/1」等はidの指定が
        // 加味され「GetProducts(int id)」がマッピングされる

        // void型の戻り値ではHTTPステータスコード:204(NoContent)を返却する  
        // ※現実的には使用しない
        [HttpPost]
        [Route("Post")]　
        public void Post() { 
        }
        // タスク
        [Route("Get2")]
        // IHttpActionResult:
        //  HttpResponseMessage（HTTPのレスポンスヘッダ＋ボディを構成するメッセージオブジェクト）
        // を返却する「タスク」を戻すExecuteAsyncメソッドをインタフェースとして持つ
        // コントローラはExecuteAsyncメソッドを実行し結果を取得。HTTPレスポンスとして利用する。
        public IHttpActionResult Get2() {
            return new TextResult("hello", Request);

        } // この行にブレークポイントを設定し実行
        // →なぜか？
        // 　　→Get2メソッドはタスクを作成し返却する事で終了してしまう。
        //       .NetFrameworkはResponseMessageを取得するため「非同期」でタスクを実行する
        // →実行順は？
        //    Get2メソッドが実行終了
        //    →Get2メソッドの結果タスクが非同期実行
        //    →タスクにより返却されたメッセージがHTTPレスポンスとして使用される
        // →何がうれしいの？
        //   Taskの非同期実行は他の処理と並列して実行される。
        //   (イメージとしては1ステップ毎だが、割り込みが入るかによって数ステップ実行する事も)
        //   ファイルやDBとのI/O待ちなど、応答待ちに時間が掛かる可能性がある場合、並列化する事で
        //   待っている間にCPUを別のスレッドやアプリケーションに譲る事ができる。
        // 非同期処理について：https://tech-lab.sios.jp/archives/15637


        [HttpGet] // Getメソッドのみを明示
        [Route("")]　// 「/」で受け取れる→ URI:/Products
        public IEnumerable<Product> GetAllProducts() {
            // HTTP レスポンスコードは 200 OK が扱われる。
            //return products;
            try {
                // 成功時は200 OKなのでそのままとし
                return products;
            } catch (Exception ex) {
                // 例えば例外が発生した場合は 500 InternalServerErrorを返却
                // ※ただし、デバッグ時にここで停止してしまうので作業効率が悪くなる
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet] // Getメソッドのみを明示
        [Route("{id:int}")] // idをURLで取得(int型に制約をつける)→ URI:.Products/{id}
        // 制約をつけると？ intに該当しないURIが404 NotFoundになる 例：Products/Abc は404応答
        public IHttpActionResult GetProduct(int id) { // int id = 10 とすると、未指定時の初期値を指定する事もできるがAPIの場合使用しない(機能公開という側面から冗長的にしないため)
            // Linqで検索し応答
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null) {
                // ポリシーにもよるが、該当データが無い場合はBadRequestと返すケースもある
                // ※一覧を取得し個別にデータを取得するのであれば発生する事はないはずなので
                return NotFound();
            }
            return Ok(product);
        }

    }
    /// <summary>
    /// HttpResponseMessageを返却するクラス
    /// コントローラ処理とレスポンス生成を切り分けるために
    /// クラスかすることがあるためサンプルとして作成
    /// ※ただし、IHttpActionResultでの応答作成が主であるため利用頻度な少ない
    /// </summary>
    public class TextResult: IHttpActionResult {
        string _value;
        HttpRequestMessage _request;

        public TextResult(string value, HttpRequestMessage request) {
            // コンストラクタでリクエスト情報を取得
            _value = value;
            _request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            // レスポンス情報を作成する
            var response = new HttpResponseMessage() {
                // コンテンツ＝メッセージボディ
                Content = new StringContent(_value)//,
                // リクエスト情報を設定
                // 　　→ リダイレクト等で使用する事がある（無くてもOK）
                //RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}
