%namespace  Mint.Parse
%parsertype Parser
%partial
%visibility public

%scanbasetype Lexer
%tokentype TokenType

%using Mint.Lex;

%YYSTYPE SyntaxNode

%token tUNKNOWN

%token  kALIAS          kAMPER          kAND            kANDDOT         kANDOP          kAPP_BEGIN      kAPP_END        kAREF
%token  kASET           kASSIGN         kASSOC          kBACKSLASH      kBACKTICK       kBEGIN          kBIN_AND        kBREAK
%token  kCASE           kCLASS          kCMP            kCOLON          kCOLON2         kCOLON3         kCOMMA          kDEF
%token  kDEFINED        kDIV            kDO             kDOT            kDOT2           kDOT3           kDO_BLOCK       kDO_COND
%token  kDO_LAMBDA      kDSTAR          kELSE           kELSIF          kEND            kENSURE         kEQ             kEQQ
%token  kFALSE          kFOR            kGEQ            kGREATER        kIF             kIF_MOD         kIN             kLAMBDA
%token  kLAMBEG         kLBRACE         kLBRACE2        kLBRACE_ARG     kLBRACK         kLBRACK2        kLEQ            kLESS
%token  kLPAREN         kLPAREN2        kLPAREN_ARG     kLSHIFT         kMATCH          kMINUS          kMODULE         kMUL
%token  kNEG            kNEQ            kNEXT           kNIL            kNL             kNMATCH         kNOT            kNOTOP
%token  kOR             kOROP           kPERCENT        kPIPE           kPLUS           kPOW            kQMARK          kRBRACE
%token  kRBRACK         kREDO           kRESCUE         kRESCUE_MOD     kRETRY          kRETURN         kRPAREN         kRSHIFT
%token  kSELF           kSEMICOLON      kSTAR           kSUPER          kTHEN           kTRUE           kUMINUS         kUMINUS_NUM
%token  kUNDEF          kUNLESS         kUNLESS_MOD     kUNTIL          kUNTIL_MOD      kUPLUS          kWHEN           kWHILE
%token  kWHILE_MOD      kXOR            kYIELD          k__ENCODING__   k__FILE__       k__LINE__       tBACK_REF       tCHAR
%token  tCONSTANT       tCVAR           tFID            tFLOAT          tGVAR           tIDENTIFIER     tIMAGINARY      tINTEGER
%token  tIVAR           tLABEL          tLABEL_END      tNTH_REF        tOP_ASGN        tQSYMBOLS_BEG   tQWORDS_BEG     tRATIONAL
%token  tREGEXP_BEG     tREGEXP_END     tSPACE          tSTRING_BEG     tSTRING_CONTENT tSTRING_DBEG    tSTRING_DEND    tSTRING_DVAR
%token  tSTRING_END     tSYMBEG         tSYMBOLS_BEG    tWORDS_BEG      tXSTRING_BEG

%nonassoc  tLOWEST
%nonassoc  kLBRACE_ARG
%nonassoc  kIF_MOD kUNLESS_MOD kWHILE_MOD kUNTIL_MOD
%left      kOR kAND
%right     kNOT
%nonassoc  kDEFINED
%right     kASSIGN tOP_ASGN
%left      kRESCUE_MOD
%right     kQMARK kCOLON
%nonassoc  kDOT2 kDOT3
%left      kOROP
%left      kANDOP
%nonassoc  kCMP kEQ kEQQ kNEQ kMATCH kNMATCH
%left      kGREATER kGEQ kLESS kLEQ
%left      kPIPE kXOR
%left      kBIN_AND
%left      kLSHIFT kRSHIFT
%left      kPLUS kMINUS
%left      kMUL kDIV kPERCENT
%right     kUMINUS_NUM kUMINUS
%right     kPOW
%right     kNOTOP kNEG kUPLUS

%%

program :
    top_compstmt
;

top_compstmt :
    top_stmts opt_terms
;

top_stmts :
    { $$ = new SyntaxNode(); } // nothing
  | top_stmt                 { $$ = new SyntaxNode($1); }
  | top_stmts terms top_stmt { $$ = $1 + $3; }
  //| error top_stmt           { $$ = new SyntaxNode($2); } // Must give error
;

top_stmt :
    stmt
  | kAPP_BEGIN kLBRACE2 top_compstmt kRBRACE { $$ = $1 + $3; }
;

bodystmt :
    compstmt opt_rescue opt_else opt_ensure
    {
        var compstmt   = $1;
        var opt_rescue = $2;
        var opt_else   = $3;
        var opt_ensure = $4;

        if(!opt_else.IsList)
        {
            if(opt_rescue.List.Count != 0)
            {
                opt_rescue += opt_else;
            }
            else
            {
                Console.WriteLine("else without rescue is useless");
            }
        }

        if(opt_ensure.List.Count != 0)
        {
            $$ = new SyntaxNode(opt_ensure.Token, compstmt, opt_rescue, opt_ensure[0]);
            break;
        }

        if(opt_rescue.List.Count != 0)
        {
            $$ = EnsureNode(compstmt, opt_rescue);
            break;
        }

        $$ = compstmt;
    }
;

compstmt :
    stmts opt_terms
;

stmts :
    { $$ = new SyntaxNode(); } // nothing
  | stmt_or_begin             { $$ = new SyntaxNode($1); }
  | stmts terms stmt_or_begin { $$ = $1 + $3; }
  //| error stmt                { $$ = new SyntaxNode($2); } // Must give error
;

stmt_or_begin :
    stmt
  | kAPP_BEGIN
    {
        throw new SyntaxError(Filename, $1.Token.Location.StartLine, "BEGIN is permitted only at toplevel");
    }
    kLBRACE2 top_compstmt kRBRACE
;

stmt :
    kALIAS fitem { Lexer.CurrentState = Lexer.FnameState; } fitem { $$ = $1 + $2 + $4; }
  | kALIAS tGVAR tGVAR     { $$ = $1 + $2 + $3; }
  | kALIAS tGVAR tBACK_REF { $$ = $1 + $2 + $3; }
  | kALIAS tGVAR tNTH_REF
    {
        throw new SyntaxError(Filename, $3.Token.Location.StartLine, "can't make alias for the number variables");
    }
  | kUNDEF undef_list     { $$ = $1 + $2; }
  | stmt kIF_MOD expr     { $$ = $2 + $3 + $1; }
  | stmt kUNLESS_MOD expr { $$ = $2 + $3 + $1; }
  | stmt kWHILE_MOD expr  { $$ = $2 + $3 + $1; }
  | stmt kUNTIL_MOD expr  { $$ = $2 + $3 + $1; }
  | stmt kRESCUE_MOD stmt { $$ = EnsureNode($1, $2 + $3); }
  | kAPP_END kLBRACE2 compstmt kRBRACE
    {
        if(inDef || inSingle)
        {
            Console.WriteLine("END in method; use at_exit");
        }
        $$ = $1 + $3;
    }
  | command_asgn
  | mlhs kASSIGN command_call                                     { $$ = $2 + $1 + $3; }
  | var_lhs tOP_ASGN command_call                                 { $$ = $2 + $1 + $3; }
  | primary kLBRACK2 opt_call_args rbracket tOP_ASGN command_call { $$ = $5 + ( $2 + $1 + $3 ) + $6; }
  | primary call_op tIDENTIFIER tOP_ASGN command_call             { $$ = $4 + ( $2 + $3 + $1 ) + $5; }
  | primary call_op tCONSTANT tOP_ASGN command_call               { $$ = $4 + ( $2 + $3 + $1 ) + $5; }
  | primary kCOLON2 tIDENTIFIER tOP_ASGN command_call             { $$ = $4 + ( $2 + $3 + $1 ) + $5; }
  | primary kCOLON2 tCONSTANT tOP_ASGN command_call               { $$ = $4 + ( $2 + $3 + $1 ) + $5; }
  | backref tOP_ASGN command_call                                 { $$ = $2 + $1 + $3; }
  | lhs kASSIGN mrhs                                              { $$ = $2 + $1 + $3; }
  | mlhs kASSIGN mrhs_arg                                         { $$ = $2 + $1 + $3; }
  | expr
;

command_asgn :
    lhs kASSIGN command_call { $$ = $2 + $1 + $3; }
  | lhs kASSIGN command_asgn { $$ = $2 + $1 + $3; }
;

expr :
    command_call
  | expr kAND expr      { $$ = $2 + $1 + $3; }
  | expr kOR expr       { $$ = $2 + $1 + $3; }
  | kNOT opt_nl expr    { $$ = $1 + $3; }
  | kNOTOP command_call { $$ = $1 + $2; }
  | arg
;

command_call :
    command
  | block_command
;

block_command :
    block_call
  | block_call call_op2 operation2 command_args { $$ = $2 + $1 + $3 + $4; }
;

cmd_brace_block :
    kLBRACE_ARG { Lexer.PushOpenScope(); } opt_block_param compstmt { Lexer.PopOpenScope(); } kRBRACE { $$ = $1 + $3 + $4; }
;

fcall :
    operation
;

command :
    fcall command_args %prec tLOWEST { $$ = CallNode($1, $2); }
  | fcall command_args cmd_brace_block
    {
      //block_dup_check($2, $3);
      $$ = CallNode($1, $2 + $3);
    }
  | primary call_op operation2 command_args %prec tLOWEST { $$ = $2 + $1 + $3 + $4; }
  | primary call_op operation2 command_args cmd_brace_block
    {
      //block_dup_check($4, $5);
      $$ = $2 + $1 + $3 + ($4 + $5);
    }
  | primary kCOLON2 operation2 command_args %prec tLOWEST { $$ = $2 + $1 + $3 + $4; }
  | primary kCOLON2 operation2 command_args cmd_brace_block
    {
      //block_dup_check($4, $5);
      $$ = $2 + $1 + $3 + ($4 + $5);
    }
  | kSUPER command_args { $$ = $1 + $2; }
  | kYIELD command_args { $$ = $1 + $2; }
  | kRETURN call_args   { $$ = $1.Append($2.List); }
  | kBREAK call_args    { $$ = $1.Append($2.List); }
  | kNEXT call_args     { $$ = $1.Append($2.List); }
;

mlhs :
    mlhs_basic
  | kLPAREN mlhs_inner rparen { $$ = $2; }
;

mlhs_inner :
    mlhs_basic
  | kLPAREN mlhs_inner rparen { $$ = $2; }
;

mlhs_basic :
    mlhs_head
  | mlhs_head mlhs_item                        { $$ = $1 + $2; }
  | mlhs_head kSTAR mlhs_node                  { $$ = $1 + ($2 + $3); }
  | mlhs_head kSTAR mlhs_node kCOMMA mlhs_post { $$ = $1 + ($2 + $3) + $5; }
  | mlhs_head kSTAR                            { $$ = $1 + $2; }
  | mlhs_head kSTAR kCOMMA mlhs_post           { $$ = $1 + $2 + $4; }
  | kSTAR mlhs_node                            { $$ = $1 + $2; }
  | kSTAR mlhs_node kCOMMA mlhs_post           { $$ = new SyntaxNode($1 + $2) + $4; }
  | kSTAR                                      { $$ = new SyntaxNode($1); }
  | kSTAR kCOMMA mlhs_post                     { $$ = new SyntaxNode($1) + $3; }
;

mlhs_item :
    mlhs_node                 { $$ = new SyntaxNode($1); }
  | kLPAREN mlhs_inner rparen { $$ = $2; }
;

mlhs_head :
    mlhs_item kCOMMA
  | mlhs_head mlhs_item kCOMMA  { $$ = $1 + $2; }
;

mlhs_post :
    mlhs_item
  | mlhs_post kCOMMA mlhs_item { $$ = $1 + $3; }
;

mlhs_node :
    user_variable { Lexer.DefineVariable($1); }
  | keyword_variable { Lexer.DefineVariable($1); }
  | primary kLBRACK2 opt_call_args rbracket  { $$ = $2 + $1 + $3; }
  | primary call_op tIDENTIFIER { $$ = $2 + $1 + $3; }
  | primary kCOLON2 tIDENTIFIER { $$ = $2 + $1 + $3; }
  | primary call_op tCONSTANT   { $$ = $2 + $1 + $3; }
  | primary kCOLON2 tCONSTANT
    {
        if(inDef || inSingle)
        {
            throw new SyntaxError(Filename, $3.Token.Location.StartLine, "dynamic constant assignment");
        }
        $$ = $2 + $1 + $3;
    }
  | kCOLON3 tCONSTANT
    {
        if(inDef || inSingle)
        {
            throw new SyntaxError(Filename, $2.Token.Location.StartLine, "dynamic constant assignment");
        }
        $$ = $1 + $2;
    }
  | backref
    {
        if($1.Token.Type == TokenType.tNTH_REF
        || $1.Token.Type == TokenType.tBACK_REF)
        {
            throw new SyntaxError(Filename, $1.Token.Location.StartLine, "Can't set variable " + $1.Token.Text);
        }
    }
;

lhs :
    user_variable { Lexer.DefineVariable($1); }
  | keyword_variable { Lexer.DefineVariable($1); }
  | primary kLBRACK2 opt_call_args rbracket { $$ = $2 + $1 + $3; }
  | primary call_op tIDENTIFIER { $$ = $2 + $1 + $3; }
  | primary kCOLON2 tIDENTIFIER { $$ = $2 + $1 + $3; }
  | primary call_op tCONSTANT   { $$ = $2 + $1 + $3; }
  | primary kCOLON2 tCONSTANT
    {
        if(inDef || inSingle)
        {
            throw new SyntaxError(Filename, $3.Token.Location.StartLine, "dynamic constant assignment");
        }
        $$ = $2 + $1 + $3;
    }
  | kCOLON3 tCONSTANT
    {
        if(inDef || inSingle)
        {
            throw new SyntaxError(Filename, $2.Token.Location.StartLine, "dynamic constant assignment");
        }
      $$ = $1 + $2;
    }
  | backref
    {
        if($1.Token.Type == TokenType.tNTH_REF
        || $1.Token.Type == TokenType.tBACK_REF)
        {
            throw new SyntaxError(Filename, $1.Token.Location.StartLine, "Can't set variable " + $1.Token.Text);
        }
    }
;

cname :
    tIDENTIFIER
    {
        throw new SyntaxError(Filename, $1.Token.Location.StartLine, "class/module name must be CONSTANT");
    }
  | tCONSTANT
;

cpath :
    kCOLON3 cname         { $$ = $1 + $2; }
  | cname
  | primary kCOLON2 cname { $$ = $2 + $1 + $3; }
;

fname :
    tIDENTIFIER
  | tCONSTANT
  | tFID
  | op { Lexer.CurrentState = Lexer.EndfnState; }
  | reswords { Lexer.CurrentState = Lexer.EndfnState; }
;

fsym :
    fname
  | symbol
;

fitem :
    fsym
  | dsym
;

undef_list :
    fitem { $$ = new SyntaxNode($1); }
  | undef_list kCOMMA { Lexer.CurrentState = Lexer.FnameState; } fitem { $$ = $1 + new SyntaxNode($4); }
;

op :
    kPIPE
  | kXOR
  | kBIN_AND
  | kCMP
  | kEQ
  | kEQQ
  | kMATCH
  | kNMATCH
  | kGREATER
  | kGEQ
  | kLESS
  | kLEQ
  | kNEQ
  | kLSHIFT
  | kRSHIFT
  | kPLUS
  | kMINUS
  | kMUL
  | kSTAR
  | kDIV
  | kPERCENT
  | kPOW
  | kDSTAR
  | kNOTOP
  | kNEG
  | kUPLUS
  | kUMINUS
  | kAREF
  | kASET
  | kBACKTICK
;

reswords :
    k__LINE__
  | k__FILE__
  | k__ENCODING__
  | kAPP_BEGIN
  | kAPP_END
  | kALIAS
  | kAND
  | kBEGIN
  | kBREAK
  | kCASE
  | kCLASS
  | kDEF
  | kDEFINED
  | kDO
  | kELSE
  | kELSIF
  | kEND
  | kENSURE
  | kFALSE
  | kFOR
  | kIN
  | kMODULE
  | kNEXT
  | kNIL
  | kNOT
  | kOR
  | kREDO
  | kRESCUE
  | kRETRY
  | kRETURN
  | kSELF
  | kSUPER
  | kTHEN
  | kTRUE
  | kUNDEF
  | kWHEN
  | kYIELD
  | kIF
  | kUNLESS
  | kWHILE
  | kUNTIL
;

arg :
    lhs kASSIGN arg                                      { $$ = $2 + $1 + $3; }
  | lhs kASSIGN arg kRESCUE_MOD arg                      { $$ = EnsureNode($2 + $1 + $3, $4 + $5); }
  | var_lhs tOP_ASGN arg                                 { $$ = $2 + $1 + $3; }
  | var_lhs tOP_ASGN arg kRESCUE_MOD arg                 { $$ = EnsureNode($2 + $1 + $3, $4 + $5); }
  | primary kLBRACK2 opt_call_args rbracket tOP_ASGN arg { $$ = $5 + ($2 + $1 + $3) + $6; }
  | primary call_op tIDENTIFIER tOP_ASGN arg             { $$ = $4 + ($2 + $1 + $3) + $5; }
  | primary call_op tCONSTANT tOP_ASGN arg               { $$ = $4 + ($2 + $1 + $3) + $5; }
  | primary kCOLON2 tIDENTIFIER tOP_ASGN arg             { $$ = $4 + ($2 + $1 + $3) + $5; }
  | primary kCOLON2 tCONSTANT tOP_ASGN arg               { $$ = $4 + ($2 + $1 + $3) + $5; }
  | kCOLON3 tCONSTANT tOP_ASGN arg                       { $$ = $3 + ($1 + $2) + $4; }
  | backref tOP_ASGN arg                                 { $$ = $2 + $1 + $3; }
  | arg kDOT2 arg                                        { $$ = $2 + $1 + $3; }
  | arg kDOT3 arg                                        { $$ = $2 + $1 + $3; }
  | arg kPLUS arg                                        { $$ = $2 + $1 + $3; }
  | arg kMINUS arg                                       { $$ = $2 + $1 + $3; }
  | arg kMUL arg                                         { $$ = $2 + $1 + $3; }
  | arg kDIV arg                                         { $$ = $2 + $1 + $3; }
  | arg kPERCENT arg                                     { $$ = $2 + $1 + $3; }
  | arg kPOW arg                                         { $$ = $2 + $1 + $3; }
  | kUMINUS_NUM simple_numeric kPOW arg                  { $$ = $3 + ($1 + $2) + $4; }
  | kUPLUS arg                                           { $$ = $1 + $2; }
  | kUMINUS arg                                          { $$ = $1 + $2; }
  | arg kPIPE arg                                        { $$ = $2 + $1 + $3; }
  | arg kXOR arg                                         { $$ = $2 + $1 + $3; }
  | arg kBIN_AND arg                                     { $$ = $2 + $1 + $3; }
  | arg kCMP arg                                         { $$ = $2 + $1 + $3; }
  | arg kGREATER arg                                     { $$ = $2 + $1 + $3; }
  | arg kGEQ arg                                         { $$ = $2 + $1 + $3; }
  | arg kLESS arg                                        { $$ = $2 + $1 + $3; }
  | arg kLEQ arg                                         { $$ = $2 + $1 + $3; }
  | arg kEQ arg                                          { $$ = $2 + $1 + $3; }
  | arg kEQQ arg                                         { $$ = $2 + $1 + $3; }
  | arg kNEQ arg                                         { $$ = $2 + $1 + $3; }
  | arg kMATCH arg                                       { $$ = $2 + $1 + $3; }
  | arg kNMATCH arg                                      { $$ = $2 + $1 + $3; }
  | kNOTOP arg                                           { $$ = $1 + $2; }
  | kNEG arg                                             { $$ = $1 + $2; }
  | arg kLSHIFT arg                                      { $$ = $2 + $1 + $3; }
  | arg kRSHIFT arg                                      { $$ = $2 + $1 + $3; }
  | arg kANDOP arg                                       { $$ = $2 + $1 + $3; }
  | arg kOROP arg                                        { $$ = $2 + $1 + $3; }
  | kDEFINED opt_nl arg                                  { $$ = $1 + $3; }
  | arg kQMARK arg opt_nl kCOLON arg                     { $$ = $2 + $1 + $3 + $6; }
  | primary
;

aref_args :
    { $$ = new SyntaxNode(); } // nothing
  | args trailer
  | args kCOMMA assocs trailer { $$ = $1 + $3; }
  | assocs trailer
;

paren_args :
    kLPAREN2 opt_call_args rparen { $$ = $2; }
;

opt_paren_args :
    { $$ = new SyntaxNode(); } // nothing
  | paren_args
;

opt_call_args :
    { $$ = new SyntaxNode(); } // nothing
  | call_args
  | args kCOMMA
  | args kCOMMA assocs kCOMMA { $$ = $1 + $3; }
  | assocs kCOMMA
;

call_args :
    command                          { $$ = new SyntaxNode($1); }
  | args opt_block_arg               { $$ = $1 + $2; }
  | assocs opt_block_arg             { $$ = $1 + $2; }
  | args kCOMMA assocs opt_block_arg { $$ = $1 + $3 + $4; }
  | block_arg                        { $$ = new SyntaxNode($1); }
;

command_args :
    {
      PushCmdarg();
      Lexer.Cmdarg.Push(true);
    }
    call_args
    {
      PopCmdarg();
      $$ = $2;
    }
;

block_arg :
  kAMPER arg { $$ = $1 + $2; }
;

opt_block_arg :
    kCOMMA block_arg  { $$ = $2; }
  | { $$ = new SyntaxNode(); } // nothing
;

args :
    arg                   { $$ = new SyntaxNode($1); }
  | kSTAR arg             { $$ = new SyntaxNode($1 + $2); }
  | args kCOMMA arg       { $$ = $1 + $3; }
  | args kCOMMA kSTAR arg { $$ = $1 + ($3 + $4); }
;

mrhs_arg :
    mrhs
  | arg
;

mrhs :
    args kCOMMA arg       { $$ = $1 + $3; }
  | args kCOMMA kSTAR arg { $$ = $1 + ($3 + $4); }
  | kSTAR arg             { $$ = new SyntaxNode($1 + $2); }
;

primary :
    literal
  | strings
  | xstring
  | regexp
  | words
  | qwords
  | symbols
  | qsymbols
  | var_ref
  | backref
  | tFID
  | kBEGIN
    {
      PushCmdarg();
      Lexer.Cmdarg = new BitStack();
    }
    bodystmt kEND { PopCmdarg(); $$ = $1.Append($3.List); }
  | kLPAREN_ARG { Lexer.CurrentState = Lexer.EndargState; } rparen { $$ = new SyntaxNode(); }
  | kLPAREN_ARG
    {
      PushCmdarg();
      Lexer.Cmdarg = new BitStack();
    }
    expr { Lexer.CurrentState = Lexer.EndargState; } rparen
    {
      PopCmdarg();
      $$ = $3;
    }
  | kLPAREN compstmt kRPAREN   { $$ = $2; }
  | primary kCOLON2 tCONSTANT  { $$ = $2 + $1 + $3; }
  | kCOLON3 tCONSTANT          { $$ = $1 + $2; }
  | kLBRACK aref_args kRBRACK  { $$ = $1.Append($2.List); }
  | kLBRACE assoc_list kRBRACE { $$ = $1.Append($2.List); }
  | kRETURN
  | kYIELD kLPAREN2 call_args rparen { $$ = $1 + $3; }
  | kYIELD kLPAREN2 rparen           { $$ = $1 + new SyntaxNode(); }
  | kYIELD
  | kDEFINED opt_nl kLPAREN2 expr rparen { $$ = $1 + $4; }
  | kNOT kLPAREN2 expr rparen { $$ = $1 + $3; }
  | kNOT kLPAREN2 rparen      { $$ = $1 + new SyntaxNode(); }
  | fcall brace_block         { $$ = CallNode($1, new SyntaxNode($2)); }
  | method_call
  | method_call brace_block
    {
        var method_call = $1;
        if(method_call.List.Count == 0)
        {
            // it''s kSUPER without parameters
            method_call += $2;
        }
        else
        {
            // the last in the list are the parameters => append to it
            var index = method_call.List.Count-1;
            ((IList<SyntaxNode>) method_call.List)[index] = method_call[index].Append($2);
        }
        $$ = method_call;
    }
  | kLAMBDA lambda
    {
        var args_body = $2;
        $$ = $1 + args_body[0] + args_body[1];
    }
  | kIF expr then compstmt if_tail kEND { $$ = $1 + $2 + $4 + $5; }
  | kUNLESS expr then compstmt opt_else kEND { $$ = $1 + $2 + $4 + $5; }
  | kWHILE { Lexer.Cond.Push(true); } expr do { Lexer.Cond.Pop(); } compstmt kEND { $$ = $1 + $3 + $6; }
  | kUNTIL { Lexer.Cond.Push(true); } expr do { Lexer.Cond.Pop(); } compstmt kEND { $$ = $1 + $3 + $6; }
  | kCASE expr opt_terms case_body kEND { $$ = $1 + $2 + $4; }
  | kCASE opt_terms case_body kEND { $$ = $1 + new SyntaxNode() + $3; }
  | kFOR for_var kIN { Lexer.Cond.Push(true); } expr do { Lexer.Cond.Pop(); } compstmt kEND
    {
        $$ = $1 + $2 + $5 + $8;
    }
  | kCLASS cpath superclass
    {
        if(inDef || inSingle)
        {
            throw new SyntaxError(Filename, $1.Token.Location.StartLine, "class definition in method body");
        }
        Lexer.PushClosedScope();
    }
    bodystmt kEND
    {
        Lexer.PopClosedScope();
        $$ = $1 + $2 + $3 + $5;
    }
  | kCLASS kLSHIFT expr
    {
        PushDef();
        inDef = false;
        PushSingle();
        inSingle = false;
        Lexer.PushClosedScope();
    }
    term bodystmt kEND
    {
        Lexer.PopClosedScope();
        PopDef();
        PopSingle();
        $$ = $1 + ($2 + $3) + new SyntaxNode() + $6;
    }
  | kMODULE cpath
    {
        if(inDef || inSingle)
        {
            throw new SyntaxError(Filename, $1.Token.Location.StartLine, "module definition in method body");
        }
        Lexer.PushClosedScope();
    }
    bodystmt kEND
    {
        Lexer.PopClosedScope();
        $$ = $1 + $2 + $4;
    }
  | kDEF fname
    {
        PushDef();
        inDef = true;
        Lexer.PushClosedScope();
    }
    f_arglist bodystmt kEND
    {
        Lexer.PopClosedScope();
        PopDef();
        $$ = $1 + new SyntaxNode() + $2 + $4 + $5;
    }
  | kDEF singleton dot_or_colon { Lexer.CurrentState = Lexer.FnameState; } fname
    {
        PushSingle();
        inSingle = true;
        Lexer.CurrentState = Lexer.EndfnState;
        Lexer.CanLabel = true;
        Lexer.PushClosedScope();
    }
    f_arglist bodystmt kEND
    {
        Lexer.PopClosedScope();
        PopSingle();
        $$ = $1 + $2 + $5 + $7 + $8;
    }
  | kBREAK
  | kNEXT
  | kREDO
  | kRETRY
;

then :
    term
  | kTHEN
  | term kTHEN
;

do :
    term
  | kDO_COND
;

if_tail :
    opt_else
  | kELSIF expr then compstmt if_tail { $$ = $1 + $2 + $4 + $5; }
;

opt_else :
    { $$ = new SyntaxNode(); } // nothing
  | kELSE compstmt { $$ = $1 + $2; }
;

for_var :
    lhs
  | mlhs
;

f_marg :
    f_norm_arg { $$ = new SyntaxNode($1); }
  | kLPAREN f_margs rparen { $$ = $2; }
;

f_marg_list :
    f_marg
  | f_marg_list kCOMMA f_marg { $$ = $1 + $3; }
;

f_margs :
    f_marg_list
  | f_marg_list kCOMMA kSTAR f_norm_arg { $$ = $1 + ($3 + $4); }
  | f_marg_list kCOMMA kSTAR f_norm_arg kCOMMA f_marg_list { $$ = $1 + ($3 + $4) + $6; }
  | f_marg_list kCOMMA kSTAR { $$ = $1 + $3; }
  | f_marg_list kCOMMA kSTAR kCOMMA f_marg_list { $$ = $1 + $3 + $5; }
  | kSTAR f_norm_arg { $$ = new SyntaxNode($1 + $2); }
  | kSTAR f_norm_arg kCOMMA f_marg_list { $$ = new SyntaxNode($1 + $2) + $4; }
  | kSTAR { $$ = new SyntaxNode($1); }
  | kSTAR kCOMMA f_marg_list { $$ = new SyntaxNode($1) + $3; }
;

block_args_tail :
    f_block_kwarg kCOMMA f_kwrest opt_f_block_arg
    {
      $$ = $1 + $3 + $4;
    }
  | f_block_kwarg opt_f_block_arg
    {
      $$ = $1 + $2;
    }
  | f_kwrest opt_f_block_arg
    {
      $$ = new SyntaxNode($1) + $2;
    }
  | f_block_arg { $$ = new SyntaxNode($1); }
;

opt_block_args_tail :
    kCOMMA block_args_tail { $$ = $2; }
  | { $$ = new SyntaxNode(); } // nothing
;

block_param :
    f_arg kCOMMA f_block_optarg kCOMMA f_rest_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $5 + $6;
    }
  | f_arg kCOMMA f_block_optarg kCOMMA f_rest_arg kCOMMA f_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $5 + $7 + $8;
    }
  | f_arg kCOMMA f_block_optarg opt_block_args_tail
    {
      $$ = $1 + $3 + $4;
    }
  | f_arg kCOMMA f_block_optarg kCOMMA f_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $5 + $6;
    }
  | f_arg kCOMMA f_rest_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $4;
    }
  | f_arg kCOMMA
  | f_arg kCOMMA f_rest_arg kCOMMA f_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $5 + $6;
    }
  | f_arg opt_block_args_tail
    {
      $$ = $1 + $2;
    }
  | f_block_optarg kCOMMA f_rest_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $4;
    }
  | f_block_optarg kCOMMA f_rest_arg kCOMMA f_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $5 + $6;
    }
  | f_block_optarg opt_block_args_tail
    {
      $$ = $1 + $2;
    }
  | f_block_optarg kCOMMA f_arg opt_block_args_tail
    {
      $$ = $1 + $3 + $4;
    }
  | f_rest_arg opt_block_args_tail
    {
      $$ = new SyntaxNode($1) + $2;
    }
  | f_rest_arg kCOMMA f_arg opt_block_args_tail
    {
      $$ = new SyntaxNode($1) + $3 + $4;
    }
  | block_args_tail
;

opt_block_param :
    { $$ = new SyntaxNode(); } // nothing
  | block_param_def { Lexer.CommandStart = true; }
;

block_param_def :
    kPIPE opt_bv_decl kPIPE             { $$ = $2; }
  | kOROP                               { $$ = new SyntaxNode(); }
  | kPIPE block_param opt_bv_decl kPIPE { $$ = $2 + $3; }
;

opt_bv_decl :
    opt_nl                            { $$ = new SyntaxNode(); }
  | opt_nl kSEMICOLON bv_decls opt_nl { $$ = $3; }
;

bv_decls :
    bvar                 { $$ = new SyntaxNode($1); }
  | bv_decls kCOMMA bvar { $$ = $1 + $3; }
;

bvar :
    tIDENTIFIER
    {
        Lexer.DefineArgument($1);
    }
  | f_bad_arg
;

lambda :
    {
        PushLParBeg();
        Lexer.LeftParenCounter = ++Lexer.ParenNest;
        Lexer.PushOpenScope();
    }
    f_larglist
    {
        PushCmdarg();
        Lexer.Cmdarg = new BitStack();
    }
    lambda_body
    {
        Lexer.PopOpenScope();
        PopLParBeg();
        PopCmdarg();
        Lexer.Cmdarg.LexPop();
        $$ = new SyntaxNode($2, $4);
    }
;

f_larglist :
    kLPAREN2 f_args opt_bv_decl kRPAREN { $$ = $2 + $3; }
  | f_args
;

lambda_body :
    kLAMBEG compstmt kRBRACE { $$ = $2; }
  | kDO_LAMBDA compstmt kEND { $$ = $2; }
;

do_block :
    kDO_BLOCK { Lexer.PushOpenScope(); } opt_block_param bodystmt { Lexer.PopOpenScope(); } kEND { $$ = $1 + $3 + $4; }
;

block_call :
    command do_block
    {
        //if (nd_type($1) == NODE_YIELD)
        //    compile_error(PARSER_ARG "block given to yield");
        //block_dup_check($1->nd_args, $2);

        var command = $1;
        var index = command.List.Count-1;
        ((IList<SyntaxNode>) command.List)[index] = command[index].Append($2);
        $$ = command;
    }
  | block_call call_op2 operation2 opt_paren_args
    {
        $$ = $2 + $1 + $3 + $4;
    }
  | block_call call_op2 operation2 opt_paren_args brace_block
    {
        // block_dup_check($4, $5);
        $$ = $2 + $1 + $3 + ($4 + $5);
    }
  | block_call call_op2 operation2 command_args do_block
    {
        // block_dup_check($4, $5);
        $$ = $2 + $1 + $3 + ($4 + $5);
    }
;

method_call :
    fcall paren_args                          { $$ = CallNode($1, $2); }
  | primary call_op operation2 opt_paren_args { $$ = $2 + $1 + $3 + $4; }
  | primary kCOLON2 operation2 paren_args     { $$ = $2 + $1 + $3 + $4; }
  | primary kCOLON2 operation3                { $$ = $2 + $1 + $3 + new SyntaxNode(); }
  | primary call_op paren_args                { $$ = $2 + $1 + new SyntaxNode() + $3; }
  | primary kCOLON2 paren_args                { $$ = $2 + $1 + new SyntaxNode() + $3; }
  | kSUPER paren_args                         { $$ = $1 + $2; }
  | kSUPER
  | primary kLBRACK2 opt_call_args rbracket   { $$ = $2 + $1 + $3; }
;

brace_block :
    kLBRACE2 { Lexer.PushOpenScope(); } opt_block_param compstmt { Lexer.PopOpenScope(); } kRBRACE { $$ = $1 + $3 + $4; }
  | kDO { Lexer.PushOpenScope(); } opt_block_param bodystmt { Lexer.PopOpenScope(); } kEND         { $$ = $1 + $3 + $4; }
;

case_body :
    kWHEN args then compstmt cases { $$ = new SyntaxNode($1 + $2 + $4) + $5; }
;

cases :
    opt_else { $$ = $1.IsList ? $1 : new SyntaxNode($1); }
  | case_body
;

opt_rescue :
    kRESCUE exc_list exc_var then compstmt opt_rescue
    {
      $$ = new SyntaxNode($1 + $2 + $3 + $5) + $6;
    }
  | { $$ = new SyntaxNode(); } // nothing
;

exc_list :
    arg
  | mrhs
  | { $$ = new SyntaxNode(); } // nothing
;

exc_var :
    kASSOC lhs { $$ = $2; }
  | { $$ = new SyntaxNode(); } // nothing
;

opt_ensure :
    kENSURE compstmt { $$ = $1 + $2; }
  | { $$ = new SyntaxNode(); } // nothing
;

literal :
    numeric
  | symbol
  | dsym
;

strings :
    string
;

string :
    tCHAR
  | string1
  | string string1 { $$ = $1.Append($2.List); }
;

string1 :
    tSTRING_BEG string_contents tSTRING_END {
        $$ = $1.Append($2.List);
        $$.Token.MergeProperties($3.Token);
    }
;

xstring :
    tXSTRING_BEG xstring_contents tSTRING_END {
        $$ = $1.Append($2.List);
        $$.Token.MergeProperties($3.Token);
    }
;

regexp :
    tREGEXP_BEG regexp_contents tREGEXP_END { $$ = $1.Append($2.List); }
;

words :
    tWORDS_BEG tSPACE tSTRING_END
  | tWORDS_BEG word_list tSTRING_END { $$ = $1.Append($2.List); }
;

word_list :
    { $$ = new SyntaxNode(); } // nothing
  | word_list word tSPACE { $$ = $1 + $2 + $3; }
;

word :
    string_content { $$ = new SyntaxNode($1); }
  | word string_content { $$ = $1 + $2; }
;

symbols :
    tSYMBOLS_BEG tSPACE tSTRING_END
  | tSYMBOLS_BEG symbol_list tSTRING_END { $$ = $1.Append($2.List); }
;

symbol_list :
    { $$ = new SyntaxNode(); } // nothing
  | symbol_list word tSPACE { $$ = $1 + $2 + $3; }
;

qwords :
    tQWORDS_BEG tSPACE tSTRING_END
  | tQWORDS_BEG qword_list tSTRING_END { $$ = $1.Append($2.List); }
;

qsymbols :
    tQSYMBOLS_BEG tSPACE tSTRING_END
  | tQSYMBOLS_BEG qsym_list tSTRING_END { $$ = $1.Append($2.List); }
;

qword_list :
    { $$ = new SyntaxNode(); } // nothing
  | qword_list tSTRING_CONTENT tSPACE { $$ = $1 + $2 + $3; }
;

qsym_list :
    { $$ = new SyntaxNode(); } // nothing
  | qsym_list tSTRING_CONTENT tSPACE { $$ = $1 + $2 + $3; }
;

string_contents :
    { $$ = new SyntaxNode(); } // nothing
  | string_contents string_content { $$ = $1 + $2; }
;

xstring_contents :
    { $$ = new SyntaxNode(); } // nothing
  | xstring_contents string_content { $$ = $1 + $2; }
;

regexp_contents :
    { $$ = new SyntaxNode(); } // nothing
  | regexp_contents string_content { $$ = $1 + $2; }
;

string_content :
    tSTRING_CONTENT
  | tSTRING_DVAR string_dvar { $$ = $1 + $2; }
  | tSTRING_DBEG
    {
      PushCond();
      Lexer.Cond = new BitStack();
      PushCmdarg();
      Lexer.Cmdarg = new BitStack();
    }
    compstmt tSTRING_DEND
    {
      PopCond();
      PopCmdarg();
      $$ = $1.Append($3.List);
    }
;

string_dvar :
    tGVAR
  | tIVAR
  | tCVAR
  | backref
;

symbol :
    tSYMBEG sym { $$ = $1 + $2; }
;

sym :
    fname
  | tIVAR
  | tGVAR
  | tCVAR
;

dsym :
  tSYMBEG xstring_contents tSTRING_END { $$ = $1.Append($2.List); }
;

numeric :
    simple_numeric
  | kUMINUS_NUM simple_numeric %prec tLOWEST { $$ = $1 + $2; }
;

simple_numeric :
    tINTEGER
  | tFLOAT
  | tRATIONAL
  | tIMAGINARY
;

user_variable :
    tIDENTIFIER
  | tIVAR
  | tGVAR
  | tCONSTANT
  | tCVAR
;

keyword_variable :
    kNIL
  | kSELF
  | kTRUE
  | kFALSE
  | k__FILE__
  | k__LINE__
  | k__ENCODING__
;

var_ref :
    user_variable
    {
      // gettable($1);
    }
  | keyword_variable
    {
      // gettable($1);
    }
;

var_lhs :
    user_variable { Lexer.DefineVariable($1); }
  | keyword_variable { Lexer.DefineVariable($1); }
;

backref :
    tNTH_REF
  | tBACK_REF
;

superclass :
    kLESS
    {
      Lexer.CurrentState = Lexer.BegState;
      Lexer.CommandStart = true;
    }
    expr term { $$ = $3; }
  | { $$ = new SyntaxNode(); } // nothing
;

f_arglist :
    kLPAREN2 f_args rparen
    {
      Lexer.CurrentState = Lexer.BegState;
      Lexer.CommandStart = true;
      $$ = $2;
    }
  | {
      PushKwarg();
      Lexer.InKwarg = true;
      Lexer.CanLabel = true;
    }
    f_args term
    {
      PopKwarg();
      Lexer.CurrentState = Lexer.BegState;
      Lexer.CommandStart = true;
      $$ = $2;
    }
;

args_tail :
    f_kwarg kCOMMA f_kwrest opt_f_block_arg { $$ = $1 + $3 + $4; }
  | f_kwarg opt_f_block_arg                 { $$ = $1 + $2; }
  | f_kwrest opt_f_block_arg                { $$ = new SyntaxNode($1) + $2; }
  | f_block_arg                             { $$ = new SyntaxNode($1); }
;

opt_args_tail :
    kCOMMA args_tail { $$ = $2; }
  | { $$ = new SyntaxNode(); } // nothing
;

f_args :
    f_arg kCOMMA f_optarg kCOMMA f_rest_arg opt_args_tail { $$ = $1 + $3 + $5 + $6; }
  | f_arg kCOMMA f_optarg kCOMMA f_rest_arg kCOMMA f_arg opt_args_tail
    {
      $$ = $1 + $3 + $5 + $7 + $8;
    }
  | f_arg kCOMMA f_optarg opt_args_tail { $$ = $1 + $3 + $4; }
  | f_arg kCOMMA f_optarg kCOMMA f_arg opt_args_tail { $$ = $1 + $3 + $5 + $6; }
  | f_arg kCOMMA f_rest_arg opt_args_tail { $$ = $1 + $3 + $4; }
  | f_arg kCOMMA f_rest_arg kCOMMA f_arg opt_args_tail { $$ = $1 + $3 + $5 + $6; }
  | f_arg opt_args_tail { $$ = $1 + $2; }
  | f_optarg kCOMMA f_rest_arg opt_args_tail { $$ = $1 + $3 + $4; }
  | f_optarg kCOMMA f_rest_arg kCOMMA f_arg opt_args_tail { $$ = $1 + $3 + $5 + $6; }
  | f_optarg opt_args_tail { $$ = $1 + $2; }
  | f_optarg kCOMMA f_arg opt_args_tail { $$ = $1 + $3 + $4; }
  | f_rest_arg opt_args_tail { $$ = new SyntaxNode($1) + $2; }
  | f_rest_arg kCOMMA f_arg opt_args_tail { $$ = new SyntaxNode($1) + $3 + $4; }
  | args_tail
  | { $$ = new SyntaxNode(); } // nothing
;

f_bad_arg :
    tCONSTANT
    {
        throw new SyntaxError(Filename, $1.Token.Location.StartLine, "formal argument cannot be a constant");
    }
  | tIVAR
    {
        throw new SyntaxError(Filename, $1.Token.Location.StartLine, "formal argument cannot be an instance variable");
    }
  | tGVAR
    {
        throw new SyntaxError(Filename, $1.Token.Location.StartLine, "formal argument cannot be a global variable");
    }
  | tCVAR
    {
        throw new SyntaxError(Filename, $1.Token.Location.StartLine, "formal argument cannot be a class variable");
    }
;

f_norm_arg :
    f_bad_arg
  | tIDENTIFIER
    {
        VerifyFormalArgument($1.Token);
        Lexer.DefineArgument($1);
    }
;

f_arg_asgn :
    f_norm_arg
;

f_arg_item :
    f_arg_asgn { $$ = new SyntaxNode($1); }
  | kLPAREN f_margs rparen { $$ = new SyntaxNode($2); }
;

f_arg :
    f_arg_item
  | f_arg kCOMMA f_arg_item { $$ = $1 + $3; }
;

f_label :
    tLABEL
    {
        VerifyFormalArgument($1.Token);
        Lexer.DefineArgument($1);
    }
;

f_kw :
    f_label arg { $$ = $1 + $2; }
  | f_label
;

f_block_kw :
    f_label primary { $$ = $1 + $2; }
  | f_label
;

f_block_kwarg :
    f_block_kw { $$ = new SyntaxNode($1); }
  | f_block_kwarg kCOMMA f_block_kw { $$ = $1 + $3; }
;

f_kwarg :
    f_kw { $$ = new SyntaxNode($1); }
  | f_kwarg kCOMMA f_kw { $$ = $1 + $3; }
;

kwrest_mark :
    kPOW
  | kDSTAR
;

f_kwrest :
    kwrest_mark tIDENTIFIER
    {
        Lexer.DefineArgument($2);
        $$ = $1 + $2;
    }
  | kwrest_mark
;

f_opt :
    f_arg_asgn kASSIGN arg { $$ = $2 + $1 + $3; }
;

f_block_opt :
    f_arg_asgn kASSIGN primary { $$ = $2 + $1 + $3; }
;

f_block_optarg :
    f_block_opt { $$ = new SyntaxNode($1); }
  | f_block_optarg kCOMMA f_block_opt { $$ = $1 + $3; }
;

f_optarg :
    f_opt { $$ = new SyntaxNode($1); }
  | f_optarg kCOMMA f_opt { $$ = $1 + $3; }
;

restarg_mark :
    kMUL
  | kSTAR
;

f_rest_arg :
    restarg_mark tIDENTIFIER
    {
        Lexer.DefineArgument($2);
        $$ = $1 + $2;
    }
  | restarg_mark
;

blkarg_mark :
    kBIN_AND
  | kAMPER
;

f_block_arg :
    blkarg_mark tIDENTIFIER
    {
        Lexer.DefineArgument($2);
        $$ = $1 + $2;
    }
;

opt_f_block_arg :
    kCOMMA f_block_arg { $$ = $2; }
  | { $$ = new SyntaxNode(); } // nothing
;

singleton :
    var_ref
  | kLPAREN2 expr rparen
    {
      //Console.WriteLine("singleton > kLPAREN2 expr rparen : #{val.inspect}");
      //if ($2 == 0) {
      //  yyerror("can't define singleton method for ().");
      //switch (nd_type($2)) {
      //  case NODE_STR:
      //  case NODE_DSTR:
      //  case NODE_XSTR:
      //  case NODE_DXSTR:
      //  case NODE_DREGX:
      //  case NODE_LIT:
      //  case NODE_ARRAY:
      //  case NODE_ZARRAY:
      //    yyerror("can't define singleton method for literals");
      //}
      $$ = $2;
    }
;

assoc_list :
    { $$ = new SyntaxNode(); } // nothing
  | assocs trailer
;

assocs :
    assoc { $$ = new SyntaxNode($1); }
  | assocs kCOMMA assoc { $$ = $1 + $3; }
;

assoc :
    arg kASSOC arg { $$ = $2 + $1 + $3; }
  | tLABEL arg { $$ = $1 + $2; }
  | tSTRING_BEG string_contents tLABEL_END arg { $$ = $3 + $2 + $4; }
  | kDSTAR arg { $$ = $1 + $2; }
;

operation :
    tIDENTIFIER
  | tCONSTANT
  | tFID
;

operation2 :
    tIDENTIFIER
  | tCONSTANT
  | tFID
  | op
;

operation3 :
    tIDENTIFIER
  | tFID
  | op
;

dot_or_colon :
    kDOT
  | kCOLON2
;

call_op :
    kDOT
  | kANDDOT
;

call_op2 :
    call_op
  | kCOLON2
;

opt_terms :
    // nothing
  | terms
;

opt_nl :
    // nothing
  | kNL
;

rparen :
    opt_nl kRPAREN
;

rbracket :
    opt_nl kRBRACK
;

trailer :
    // nothing
  | kNL
  | kCOMMA
;

term :
    kSEMICOLON { yyerrok(); }
  | kNL
;

terms :
    term
  | terms kSEMICOLON { yyerrok(); }
;

