using System;
using System.IO;

namespace Translator {
    public class Sintatic {
        private Token _LA; 
        private Lexer _lexer;

        public Sintatic(Lexer lexer){
            _lexer = lexer;
            _LA = lexer.GetNextToken();
        }


        // Verifica se a entrada é válida
        public int Parse(){
            Console.WriteLine("Entrou no PARSE");
            var res = E();
            
            if(_LA.Type != ETokenType.EOF){
                throw new Exception("ERRO de sintaxe: " + _LA.Type + " não esperado, esperava EOF");
            }
            else {
                Console.WriteLine("Entrada válida");
            }
            Console.WriteLine("Resultado: " + res);
            return res;
        }

        public void Match(ETokenType token){
            // Verificar se o token é o esperado
            Console.WriteLine("LA: " + _LA);
            if(_LA.Type == token){
                //Console.WriteLine("Token: " + _LA + " reconhecido");
                _LA = _lexer.GetNextToken();
                //Console.WriteLine("Proximo Token: " + _LA + " a ser analisado");
            }else{
                throw new Exception("Erro de sintaxe");
            }
        }

        // public void Teste(){
        //     if(_LA.Type == ETokenType.EOL){
        //         Match(ETokenType.EOL);
        //     }
        // }

        public void Prog(){ // Prog: Line A
            Line();
            A();
            
        }

        public void Line(){ // Line: stmt EOL
            Stmt();
            Match(ETokenType.EOL);
        }

        public void A(){ //A: EOF | Prog
            if(_LA.Type==ETokenType.EOF){
                Match(ETokenType.EOF);
            }
            else{
                Prog();
            }
        }

        public void Stmt(){ //Stmt: in | out | atrib
            if(_LA.Type==ETokenType.INPUT){
                In();
            }
            else if(_LA.Type==ETokenType.OUTPUT){
                Out();
            }
            else if(_LA.Type==ETokenType.VAR){
                Atrib();
            }
            else{
                throw new Exception("ERRO de sintaxe: " + _LA.Type + " não esperado, esperava INPUT, OUTPUT ou VAR");
            }
        }

        public void In(){ //in: INPUT VAR
            Match(ETokenType.INPUT);
            Match(ETokenType.VAR);
        }

        public void Out(){ //out    : OUTPUT VAR
            Match(ETokenType.OUTPUT);
            Match(ETokenType.VAR);
        }

        public void Atrib(){//atrib  : VAR AT expr
            Match(ETokenType.VAR);
            Match(ETokenType.AT);
            var res = E();
            //Colocar a variavel na tabela

            //...
            //cpa colocar ess throw
            Console.WriteLine("Resultado: " + res);
        }


        public int E(){ //expr   : term Y
            //Console.WriteLine("Estado: E ->  LA:" + _LA.Value);
            var res1 = T();
            var res2 = X(res1);
            Console.WriteLine("Estou no E e T é: " + res1 + " e X é: " + res2);
            return res2;
        }

        public int X(int t){ //X      : vazio | + expr | - expr
            //Console.WriteLine("Estado: X ->  LA:" + _LA.Value);
            if(_LA.Type==ETokenType.SUM){
                Match(ETokenType.SUM);
                var res = E();
                Console.WriteLine("Estou no X(+) e t é: " + t + " e E é: " + res);
                return t + res;
            }
            else if(_LA.Type==ETokenType.SUB){
                Match(ETokenType.SUB);
                var res = E();
                Console.WriteLine("Estou no X(-) e t é: " + t + " e E é: " + res);
                return t - res;
            }
            
            else{
                if(_LA.Type!=ETokenType.EOF && _LA.Type!=ETokenType.CLOSE){
                    throw new Exception("ERRO de sintaxe: " + _LA.Type + " não esperado, esperava + ou -");
                }
            }
            return t;
        }

        public int T(){ //term   : factZ
            //Console.WriteLine("Estado: T ->  LA:" + _LA.Value);
            var res1 = F();
            var res2 = Y(res1);
            Console.WriteLine("Estou no T e F é: " + res1 + " e Y é: " + res2);
            return res2;
        }

        public int Y(int t){ // Y     : vazio | * term | / term
            //Console.WriteLine("Estado: Y ->  LA:" + _LA.Value);
            if(_LA.Type==ETokenType.MUL){
                Match(ETokenType.MUL);
                var res = E();
                Console.WriteLine("Estou no Y(*) e t é: " + t + " e E é: " + res);
                return t * res;

            }
            else if(_LA.Type==ETokenType.DIV){
                Match(ETokenType.DIV);
                var res = E();
                Console.WriteLine("Estou no X(/) e t é: " + t + " e E é: " + res);
                return t / res;
            }
            else{
                if(_LA.Type!=ETokenType.EOF && _LA.Type!=ETokenType.CLOSE && _LA.Type!=ETokenType.SUM && _LA.Type!=ETokenType.SUB){
                    throw new Exception("ERRO de sintaxe: " + _LA.Type + " não esperado, esperava * ou /");
                }
            }
            return t;
        }

        public int F(){ //fact   : NUM | VAR | OE expr CE
            Console.WriteLine("Estado: F ->  LA:" + _LA.Value);
            if(_LA.Type==ETokenType.NUMBER){
                var res = _LA.Value;
                Match(ETokenType.NUMBER);
                Console.WriteLine("Estou no F e LA é: " + res);
                return res;
            }
            else if(_LA.Type==ETokenType.OPEN){
                Match(ETokenType.OPEN);
                var res = E();
                Match(ETokenType.CLOSE);
                Console.WriteLine("Estou no F e E é: " + res);
                return res;
            }
            else{
                if(_LA.Type!=ETokenType.EOF && _LA.Type!=ETokenType.CLOSE && _LA.Type!=ETokenType.SUM && _LA.Type!=ETokenType.SUB && _LA.Type!=ETokenType.MUL && _LA.Type!=ETokenType.DIV){
                    throw new Exception("ERRO de sintaxe: " + _LA.Type + " não esperado, esperava número ou (");
                }
            }
            return 0;
        }
    }
}
