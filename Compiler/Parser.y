﻿%output=@"Parser.y.cs"

%namespace  mint.Compiler
%parsertype Parser
%partial
%visibility internal

%using mint.types

%scanbasetype Lexer

%YYSTYPE object

%token
    keyword_class
    keyword_module
    keyword_def
    keyword_undef
    keyword_begin
    keyword_rescue
    keyword_ensure
    keyword_end
    keyword_if
    keyword_unless
    keyword_then
    keyword_elsif
    keyword_else
    keyword_case
    keyword_when
    keyword_while
    keyword_until
    keyword_for
    keyword_break
    keyword_next
    keyword_redo
    keyword_retry
    keyword_in
    keyword_do
    keyword_do_cond
    keyword_do_block
    keyword_do_LAMBDA
    keyword_return
    keyword_yield
    keyword_super
    keyword_self
    keyword_nil
    keyword_true
    keyword_false
    keyword_and
    keyword_or
    keyword_not
    modifier_if
    modifier_unless
    modifier_while
    modifier_until
    modifier_rescue
    keyword_alias
    keyword_defined
    keyword_BEGIN
    keyword_END
    keyword__LINE__
    keyword__FILE__
    keyword__ENCODING__

%token <id>   tIDENTIFIER tFID tGVAR tIVAR tCONSTANT tCVAR tLABEL
%token <node> tINTEGER tFLOAT tRATIONAL tIMAGINARY tSTRING_CONTENT tCHAR
%token <node> tNTH_REF tBACK_REF
%token <num>  tREGEXP_END

%type <node> singleton strings string string1 xstring regexp
%type <node> string_contents xstring_contents regexp_contents string_content
%type <node> words symbols symbol_list qwords qsymbols word_list qword_list qsym_list word
%type <node> literal numeric simple_numeric dsym cpath
%type <node> top_compstmt top_stmts top_stmt
%type <node> bodystmt compstmt stmts stmt_or_begin stmt expr arg primary command command_call method_call
%type <node> expr_value arg_value primary_value fcall
%type <node> if_tail opt_else case_body cases opt_rescue exc_list exc_var opt_ensure
%type <node> args call_args opt_call_args
%type <node> paren_args opt_paren_args args_tail opt_args_tail block_args_tail opt_block_args_tail
%type <node> command_args aref_args opt_block_arg block_arg var_ref var_lhs
%type <node> command_asgn mrhs mrhs_arg superclass block_call block_command
%type <node> f_block_optarg f_block_opt
%type <node> f_arglist f_args f_arg f_arg_item f_optarg f_marg f_marg_list f_margs
%type <node> assoc_list assocs assoc undef_list backref string_dvar for_var
%type <node> block_param opt_block_param block_param_def f_opt
%type <node> f_kwarg f_kw f_block_kwarg f_block_kw
%type <node> bv_decls opt_bv_decl bvar
%type <node> lambda f_larglist lambda_body
%type <node> brace_block cmd_brace_block do_block lhs none fitem
%type <node> mlhs mlhs_head mlhs_basic mlhs_item mlhs_node mlhs_post mlhs_inner
%type <id>   fsym keyword_variable user_variable sym symbol operation operation2 operation3
%type <id>   cname fname op f_rest_arg f_block_arg opt_f_block_arg f_norm_arg f_bad_arg
%type <id>   f_kwrest f_label f_arg_asgn call_op call_op2

%token END_OF_INPUT   "end-of-input"
%token tUPLUS         "unary+"
%token tUMINUS        "unary-"
%token tPOW           "**"
%token tCMP           "<=>"
%token tEQ            "=="
%token tEQQ           "==="
%token tNEQ           "!="
%token tGEQ           ">="
%token tLEQ           "<="
%token tANDOP         "&&"
%token tOROP          "||"
%token tMATCH         "=~"
%token tNMATCH        "!~"
%token tDOT2          ".."
%token tDOT3          "..."
%token tAREF          "[]"
%token tASET          "[]="
%token tLSHFT         "<<"
%token tRSHFT         ">>"
%token tANDDOT        "&."
%token tCOLON2        "::"
%token tCOLON3        ":: at EXPR_BEG"
%token <id> tOP_ASGN                   /* +=, -=  etc. */
%token tASSOC         "=>"
%token tLPAREN        "("
%token tLPAREN_ARG    "( arg"
%token tRPAREN        ")"
%token tLBRACK        "["
%token tLBRACE        "{"
%token tLBRACE_ARG    "{ arg"
%token tSTAR          "*"
%token tDSTAR         "**arg"
%token tAMPER         "&"
%token tLAMBDA        "->"
%token tSYMBEG tSTRING_BEG tXSTRING_BEG tREGEXP_BEG tWORDS_BEG tQWORDS_BEG tSYMBOLS_BEG tQSYMBOLS_BEG
%token tSTRING_DBEG tSTRING_DEND tSTRING_DVAR tSTRING_END tLAMBEG tLABEL_END

%nonassoc tLOWEST
%nonassoc tLBRACE_ARG

%nonassoc  modifier_if modifier_unless modifier_while modifier_until
%left      keyword_or keyword_and
%right     keyword_not
%nonassoc  keyword_defined
%right     '=' tOP_ASGN
%left      modifier_rescue
%right     '?' ':'
%nonassoc  tDOT2 tDOT3
%left      tOROP
%left      tANDOP
%nonassoc  tCMP tEQ tEQQ tNEQ tMATCH tNMATCH
%left      '>' tGEQ '<' tLEQ
%left      '|' '^'
%left      '&'
%left      tLSHFT tRSHFT
%left      '+' '-'
%left      '*' '/' '%'
%right     tUMINUS_NUM tUMINUS
%right     tPOW
%right     '!' '~' tUPLUS

%token tLAST_TOKEN

%%

program
	: top_compstmt
;

top_compstmt
	: top_stmts opt_terms
;

top_stmts : none
	| top_stmt
	| top_stmts terms top_stmt
	| error top_stmt
;

top_stmt
	: stmt
	| keyword_BEGIN '{' top_compstmt '}'
;

bodystmt
	: compstmt opt_rescue opt_else opt_ensure
;

compstmt
	: stmts opt_terms
;

stmts
	: none
	| stmt_or_begin
	| stmts terms stmt_or_begin
	| error stmt
;

stmt_or_begin
	: stmt
	| keyword_BEGIN '{' top_compstmt '}'
;

stmt
	: keyword_alias fitem fitem
	| keyword_alias tGVAR tGVAR
	| keyword_alias tGVAR tBACK_REF
	| keyword_alias tGVAR tNTH_REF
	| keyword_undef undef_list
	| stmt modifier_if expr_value
	| stmt modifier_unless expr_value
	| stmt modifier_while expr_value
	| stmt modifier_until expr_value
	| stmt modifier_rescue stmt
	| keyword_END '{' compstmt '}'
	| command_asgn
	| mlhs '=' command_call
	| var_lhs tOP_ASGN command_call
	| primary_value '[' opt_call_args rbracket tOP_ASGN command_call
	| primary_value call_op tIDENTIFIER tOP_ASGN command_call
	| primary_value call_op tCONSTANT tOP_ASGN command_call
	| primary_value tCOLON2 tCONSTANT tOP_ASGN command_call
	| primary_value tCOLON2 tIDENTIFIER tOP_ASGN command_call
	| backref tOP_ASGN command_call
	| lhs '=' mrhs
	| mlhs '=' mrhs_arg
	| expr
;

command_asgn
	: lhs '=' command_call
	| lhs '=' command_asgn
;

expr
	: command_call
	| expr keyword_and expr
	| expr keyword_or expr
	| keyword_not opt_nl expr
	| '!' command_call
	| arg
;

expr_value
	: expr
;

command_call
	: command
	| block_command
;

block_command
	: block_call
	| block_call call_op2 operation2 command_args
;

cmd_brace_block
	: tLBRACE_ARG opt_block_param compstmt '}'
;

fcall
	: operation
;

command
	: fcall command_args       %prec tLOWEST
	| fcall command_args cmd_brace_block
	| primary_value call_op operation2 command_args	%prec tLOWEST
	| primary_value call_op operation2 command_args cmd_brace_block
	| primary_value tCOLON2 operation2 command_args	%prec tLOWEST
	| primary_value tCOLON2 operation2 command_args cmd_brace_block
	| keyword_super command_args
	| keyword_yield command_args
	| keyword_return call_args
	| keyword_break call_args
	| keyword_next call_args
;

mlhs
	: mlhs_basic
	| tLPAREN mlhs_inner rparen
;

mlhs_inner
	: mlhs_basic
	| tLPAREN mlhs_inner rparen
;

mlhs_basic
	: mlhs_head
	| mlhs_head mlhs_item
	| mlhs_head tSTAR mlhs_node
	| mlhs_head tSTAR mlhs_node ',' mlhs_post
	| mlhs_head tSTAR
	| mlhs_head tSTAR ',' mlhs_post
	| tSTAR mlhs_node
	| tSTAR mlhs_node ',' mlhs_post
	| tSTAR
	| tSTAR ',' mlhs_post
;

mlhs_item
	: mlhs_node
	| tLPAREN mlhs_inner rparen
;

mlhs_head
	: mlhs_item ','
	| mlhs_head mlhs_item ','
;

mlhs_post
	: mlhs_item
	| mlhs_post ',' mlhs_item
;

mlhs_node
	: user_variable
	| keyword_variable
	| primary_value '[' opt_call_args rbracket
	| primary_value call_op tIDENTIFIER
	| primary_value tCOLON2 tIDENTIFIER
	| primary_value call_op tCONSTANT
	| primary_value tCOLON2 tCONSTANT
	| tCOLON3 tCONSTANT
	| backref
;

lhs
	: user_variable
	| keyword_variable
	| primary_value '[' opt_call_args rbracket
	| primary_value call_op tIDENTIFIER
	| primary_value tCOLON2 tIDENTIFIER
	| primary_value call_op tCONSTANT
	| primary_value tCOLON2 tCONSTANT
	| tCOLON3 tCONSTANT
	| backref
;

cname
	: tIDENTIFIER
	| tCONSTANT
;

cpath
	: tCOLON3 cname
	| cname
	| primary_value tCOLON2 cname
;

fname
	: tIDENTIFIER
	| tCONSTANT
	| tFID
	| op
	| reswords
;

fsym
	: fname
	| symbol
;

fitem
	: fsym
	| dsym
;

undef_list
	: fitem
	| undef_list ',' fitem
;

op
	: '|'		
	| '^'		
	| '&'		
	| tCMP		
	| tEQ		
	| tEQQ		
	| tMATCH	
	| tNMATCH	
	| '>'		
	| tGEQ		
	| '<'		
	| tLEQ		
	| tNEQ		
	| tLSHFT	
	| tRSHFT	
	| '+'		
	| '-'		
	| '*'		
	| tSTAR		
	| '/'		
	| '%'		
	| tPOW		
	| tDSTAR	
	| '!'		
	| '~'		
	| tUPLUS	
	| tUMINUS	
	| tAREF		
	| tASET		
	| '`'		
;

reswords
	: keyword__LINE__
	| keyword__FILE__
	| keyword__ENCODING__
	| keyword_BEGIN 
	| keyword_END
	| keyword_alias 
	| keyword_and 
	| keyword_begin
	| keyword_break 
	| keyword_case 
	| keyword_class 
	| keyword_def
	| keyword_defined 
	| keyword_do 
	| keyword_else 
	| keyword_elsif
	| keyword_end 
	| keyword_ensure 
	| keyword_false
	| keyword_for 
	| keyword_in 
	| keyword_module 
	| keyword_next
	| keyword_nil 
	| keyword_not 
	| keyword_or 
	| keyword_redo
	| keyword_rescue 
	| keyword_retry 
	| keyword_return 
	| keyword_self
	| keyword_super 
	| keyword_then 
	| keyword_true 
	| keyword_undef
	| keyword_when 
	| keyword_yield 
	| keyword_if 
	| keyword_unless
	| keyword_while 
	| keyword_until
;

arg
	: lhs '=' arg
	| lhs '=' arg modifier_rescue arg
	| var_lhs tOP_ASGN arg
	| var_lhs tOP_ASGN arg modifier_rescue arg
	| primary_value '[' opt_call_args rbracket tOP_ASGN arg
	| primary_value call_op tIDENTIFIER tOP_ASGN arg
	| primary_value call_op tCONSTANT tOP_ASGN arg
	| primary_value tCOLON2 tIDENTIFIER tOP_ASGN arg
	| primary_value tCOLON2 tCONSTANT tOP_ASGN arg
	| tCOLON3 tCONSTANT tOP_ASGN arg
	| backref tOP_ASGN arg
	| arg tDOT2 arg
	| arg tDOT3 arg
	| arg '+' arg
	| arg '-' arg
	| arg '*' arg
	| arg '/' arg
	| arg '%' arg
	| arg tPOW arg
	| tUMINUS_NUM simple_numeric tPOW arg
	| tUPLUS arg
	| tUMINUS arg
	| arg '|' arg
	| arg '^' arg
	| arg '&' arg
	| arg tCMP arg
	| arg '>' arg
	| arg tGEQ arg
	| arg '<' arg
	| arg tLEQ arg
	| arg tEQ arg
	| arg tEQQ arg
	| arg tNEQ arg
	| arg tMATCH arg
	| arg tNMATCH arg
	| '!' arg
	| '~' arg
	| arg tLSHFT arg
	| arg tRSHFT arg
	| arg tANDOP arg
	| arg tOROP arg
	| keyword_defined opt_nl arg
	| arg '?' arg opt_nl ':' arg
	| primary
;

arg_value
	: arg
;

aref_args
	: none
	| args trailer
	| args ',' assocs trailer
	| assocs trailer
;

paren_args
	: '(' opt_call_args rparen
;

opt_paren_args
	: none
	| paren_args
;

opt_call_args
	: none
	| call_args
	| args ','
	| args ',' assocs ','
	| assocs ','
;

call_args
	: command
	| args opt_block_arg
	| assocs opt_block_arg
	| args ',' assocs opt_block_arg
	| block_arg
;

command_args
	:  call_args
;

block_arg
	: tAMPER arg_value
;

opt_block_arg
	: ',' block_arg
	| none
;

args
	: arg_value
	| tSTAR arg_value
	| args ',' arg_value
	| args ',' tSTAR arg_value
;

mrhs_arg
	: mrhs
	| arg_value
;

mrhs
	: args ',' arg_value
	| args ',' tSTAR arg_value
	| tSTAR arg_value
;

primary
	: literal
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
	| k_begin bodystmt k_end
	| tLPAREN_ARG rparen
	| tLPAREN_ARG expr rparen
	| tLPAREN compstmt ')'
	| primary_value tCOLON2 tCONSTANT
	| tCOLON3 tCONSTANT
	| tLBRACK aref_args ']'
	| tLBRACE assoc_list '}'
	| keyword_return
	| keyword_yield '(' call_args rparen
	| keyword_yield '(' rparen
	| keyword_yield
	| keyword_defined opt_nl '(' expr rparen
	| keyword_not '(' expr rparen
	| keyword_not '(' rparen
	| fcall brace_block
	| method_call
	| method_call brace_block
	| tLAMBDA lambda
	| k_if expr_value then compstmt if_tail k_end
	| k_unless expr_value then compstmt opt_else k_end
	| k_while expr_value do compstmt k_end
	| k_until expr_value do compstmt k_end
	| k_case expr_value opt_terms case_body k_end
	| k_case opt_terms case_body k_end
	| k_for for_var keyword_in expr_value do compstmt k_end
	| k_class cpath superclass bodystmt k_end
	| k_class tLSHFT expr term bodystmt k_end
	| k_module cpath bodystmt k_end
	| k_def fname f_arglist bodystmt k_end
	| k_def singleton dot_or_colon fname f_arglist bodystmt k_end
	| keyword_break
	| keyword_next
	| keyword_redo
	| keyword_retry
;

primary_value
	: primary
;

k_begin
	: keyword_begin
;

k_if
	: keyword_if
;

k_unless
	: keyword_unless
;

k_while
	: keyword_while
;

k_until
	: keyword_until
;

k_case	
	: keyword_case
;

k_for	
	: keyword_for
;

k_class	
	: keyword_class
;

k_module
	: keyword_module
;

k_def	
	: keyword_def
;

k_end	
	: keyword_end
;

then	
	: term
	| keyword_then
	| term keyword_then
;

do	
	: term
	| keyword_do_cond
;

if_tail	
	: opt_else
	| keyword_elsif expr_value then compstmt if_tail
;

opt_else
	: none
	| keyword_else compstmt
;

for_var	
	: lhs
	| mlhs
;

f_marg	
	: f_norm_arg
	| tLPAREN f_margs rparen
;

f_marg_list
	: f_marg
	| f_marg_list ',' f_marg
;

f_margs	
	: f_marg_list
	| f_marg_list ',' tSTAR f_norm_arg
	| f_marg_list ',' tSTAR f_norm_arg ',' f_marg_list
	| f_marg_list ',' tSTAR
	| f_marg_list ',' tSTAR ',' f_marg_list
	| tSTAR f_norm_arg
	| tSTAR f_norm_arg ',' f_marg_list
	| tSTAR
	| tSTAR ',' f_marg_list
;


block_args_tail
	: f_block_kwarg ',' f_kwrest opt_f_block_arg
	| f_block_kwarg opt_f_block_arg
	| f_kwrest opt_f_block_arg
	| f_block_arg
;

opt_block_args_tail
 : ',' block_args_tail
	| /* none */
;

block_param
	: f_arg ',' f_block_optarg ',' f_rest_arg opt_block_args_tail
	| f_arg ',' f_block_optarg ',' f_rest_arg ',' f_arg opt_block_args_tail
	| f_arg ',' f_block_optarg opt_block_args_tail
	| f_arg ',' f_block_optarg ',' f_arg opt_block_args_tail
	| f_arg ',' f_rest_arg opt_block_args_tail
	| f_arg ','
	| f_arg ',' f_rest_arg ',' f_arg opt_block_args_tail
	| f_arg opt_block_args_tail
	| f_block_optarg ',' f_rest_arg opt_block_args_tail
	| f_block_optarg ',' f_rest_arg ',' f_arg opt_block_args_tail
	| f_block_optarg opt_block_args_tail
	| f_block_optarg ',' f_arg opt_block_args_tail
	| f_rest_arg opt_block_args_tail
	| f_rest_arg ',' f_arg opt_block_args_tail
	| block_args_tail
;

opt_block_param
	: none
	| block_param_def
;

block_param_def
	: '|' opt_bv_decl '|'
	| tOROP
	| '|' block_param opt_bv_decl '|'
;


opt_bv_decl
	: opt_nl
	| opt_nl ';' bv_decls opt_nl
;

bv_decls
	: bvar
	| bv_decls ',' bvar
;

bvar	
	: tIDENTIFIER
	| f_bad_arg
;

lambda	
	:   f_larglist lambda_body
;

f_larglist
	: '(' f_args opt_bv_decl ')'
	| f_args
;

lambda_body
	: tLAMBEG compstmt '}'
	| keyword_do_LAMBDA compstmt keyword_end
;

do_block
	: keyword_do_block
		opt_block_param
		compstmt
		keyword_end
;

block_call
	: command do_block
	| block_call call_op2 operation2 opt_paren_args
	| block_call call_op2 operation2 opt_paren_args brace_block
	| block_call call_op2 operation2 command_args do_block
;

method_call
	: fcall paren_args
	| primary_value call_op operation2 opt_paren_args
	| primary_value tCOLON2 operation2 paren_args
	| primary_value tCOLON2 operation3
	| primary_value call_op paren_args
	| primary_value tCOLON2 paren_args
	| keyword_super paren_args
	| keyword_super
	| primary_value '[' opt_call_args rbracket
;

brace_block
	: '{' opt_block_param compstmt '}'
	| keyword_do opt_block_param compstmt keyword_end
;

case_body
	: keyword_when args then compstmt cases
;

cases	
	: opt_else
	| case_body
;

opt_rescue
	: keyword_rescue exc_list exc_var then compstmt opt_rescue
	| none
;

exc_list
	: arg_value
	| mrhs
	| none
;

exc_var	
	: tASSOC lhs
	| none
;

opt_ensure
	: keyword_ensure compstmt
	| none
;

literal	
	: numeric
	| symbol
	| dsym
;

strings	
	: string
;

string	
	: tCHAR
	| string1
	| string string1
;

string1	
	: tSTRING_BEG string_contents tSTRING_END
;

xstring	
	: tXSTRING_BEG xstring_contents tSTRING_END
;

regexp	
	: tREGEXP_BEG regexp_contents tREGEXP_END
;

words	
	: tWORDS_BEG ' ' tSTRING_END
	| tWORDS_BEG word_list tSTRING_END
;

word_list
	: /* none */
	| word_list word ' '
;

word	
	: string_content
	| word string_content
;

symbols
	: tSYMBOLS_BEG ' ' tSTRING_END
	| tSYMBOLS_BEG symbol_list tSTRING_END
;

symbol_list
	: /* none */
	| symbol_list word ' '
;

qwords	
	: tQWORDS_BEG ' ' tSTRING_END
	| tQWORDS_BEG qword_list tSTRING_END
;

qsymbols
	: tQSYMBOLS_BEG ' ' tSTRING_END
	| tQSYMBOLS_BEG qsym_list tSTRING_END
;

qword_list
	: /* none */
	| qword_list tSTRING_CONTENT ' '
;

qsym_list
	: /* none */
	| qsym_list tSTRING_CONTENT ' '
;

string_contents
 : /* none */
	| string_contents string_content
;

xstring_contents
	: /* none */
	| xstring_contents string_content
;

regexp_contents
	: /* none */
	| regexp_contents string_content
;

string_content
	: tSTRING_CONTENT
	| tSTRING_DVAR string_dvar
	| tSTRING_DBEG compstmt tSTRING_DEND
;

string_dvar
	: tGVAR
	| tIVAR
	| tCVAR
	| backref
;

symbol	
	: tSYMBEG sym
;

sym	
	: fname
	| tIVAR
	| tGVAR
	| tCVAR
;

dsym	
	: tSYMBEG xstring_contents tSTRING_END
;

numeric 
	: simple_numeric
	| tUMINUS_NUM simple_numeric   %prec tLOWEST
;

simple_numeric
	: tINTEGER
	| tFLOAT
	| tRATIONAL
	| tIMAGINARY
;

user_variable
	: tIDENTIFIER
	| tIVAR
	| tGVAR
	| tCONSTANT
	| tCVAR
;

keyword_variable
	: keyword_nil
	| keyword_self
	| keyword_true
	| keyword_false
	| keyword__FILE__
	| keyword__LINE__
	| keyword__ENCODING__
;

var_ref	
	: user_variable
	| keyword_variable
;

var_lhs	
	: user_variable
	| keyword_variable
;

backref	
	: tNTH_REF
	| tBACK_REF
;

superclass
	: '<' expr_value term
	| /* none */
;

f_arglist
	: '(' f_args rparen
	| f_args term
;

args_tail
	: f_kwarg ',' f_kwrest opt_f_block_arg
	| f_kwarg opt_f_block_arg
	| f_kwrest opt_f_block_arg
	| f_block_arg
;

opt_args_tail
	: ',' args_tail
	| /* none */
;

f_args	
	: f_arg ',' f_optarg ',' f_rest_arg opt_args_tail
	| f_arg ',' f_optarg ',' f_rest_arg ',' f_arg opt_args_tail
	| f_arg ',' f_optarg opt_args_tail
	| f_arg ',' f_optarg ',' f_arg opt_args_tail
	| f_arg ',' f_rest_arg opt_args_tail
	| f_arg ',' f_rest_arg ',' f_arg opt_args_tail
	| f_arg opt_args_tail
	| f_optarg ',' f_rest_arg opt_args_tail
	| f_optarg ',' f_rest_arg ',' f_arg opt_args_tail
	| f_optarg opt_args_tail
	| f_optarg ',' f_arg opt_args_tail
	| f_rest_arg opt_args_tail
	| f_rest_arg ',' f_arg opt_args_tail
	| args_tail
	| /* none */
;

f_bad_arg
	: tCONSTANT
	| tIVAR
	| tGVAR
	| tCVAR
;

f_norm_arg
	: f_bad_arg
	| tIDENTIFIER
;

f_arg_asgn
	: f_norm_arg
;

f_arg_item
	: f_arg_asgn
	| tLPAREN f_margs rparen
;

f_arg	
	: f_arg_item
	| f_arg ',' f_arg_item
;


f_label 
	: tLABEL
;

f_kw	
	: f_label arg_value
	| f_label
;

f_block_kw
	: f_label primary_value
	| f_label
	;

f_block_kwarg
	: f_block_kw
	| f_block_kwarg ',' f_block_kw
;


f_kwarg	
	: f_kw
	| f_kwarg ',' f_kw
;

kwrest_mark
	: tPOW
	| tDSTAR
;

f_kwrest
	: kwrest_mark tIDENTIFIER
	| kwrest_mark
;

f_opt	
	: f_arg_asgn '=' arg_value
;

f_block_opt
	: f_arg_asgn '=' primary_value
;

f_block_optarg
	: f_block_opt
	| f_block_optarg ',' f_block_opt
;

f_optarg
	: f_opt
	| f_optarg ',' f_opt
;

restarg_mark
	: '*'
	| tSTAR
;

f_rest_arg
	: restarg_mark tIDENTIFIER
	| restarg_mark
;

blkarg_mark
	: '&'
	| tAMPER
;

f_block_arg
	: blkarg_mark tIDENTIFIER
;

opt_f_block_arg
	: ',' f_block_arg
	| none
;

singleton
	: var_ref
	| '(' expr rparen
;

assoc_list
	: none
	| assocs trailer
;

assocs	
	: assoc
	| assocs ',' assoc
;

assoc	
	: arg_value tASSOC arg_value
	| tLABEL arg_value
	| tSTRING_BEG string_contents tLABEL_END arg_value
	| tDSTAR arg_value
;

operation
	: tIDENTIFIER
	| tCONSTANT
	| tFID
;

operation2
	: tIDENTIFIER
	| tCONSTANT
	| tFID
	| op
;

operation3
	: tIDENTIFIER
	| tFID
	| op
;

dot_or_colon
	: '.'
	| tCOLON2
;

call_op 
	: '.'
	| tANDDOT
;

call_op2
	: call_op
	| tCOLON2
;

opt_terms
	: /* none */
	| terms
;

opt_nl	
	: /* none */
	| '\n'
;

rparen	
	: opt_nl ')'
;

rbracket
	: opt_nl ']'
;

trailer	
	: /* none */
	| '\n'
	| ','
;

term	
	: ';'
	| '\n'
;

terms	
	: term
	| terms ';'
;

none	
	: /* none */
;

%%