module.exports = grammar({
  name: 'jacco',

  word: $ => $.IDENT,

  rules: {
    source_file: $ => seq(
      repeat(';'),
      optional($._semi),
    ),

    IDENT: _ => /[A-Za-z_][0-9A-Za-z_-]*/,

    _ty: $ => $.IDENT,

    _pat: $ => $.IDENT,

    group_expr: $ => seq('(', $._expr, ')'),

    array_list_expr: $ => seq(
      '[',
      optional(seq(
        $._expr,
        repeat(seq(
          ',',
          $._expr,
        )),
        optional(','),
      )),
      ']',
    ),

    array_replicate_expr: $ => seq(
      '[',
      $._expr,
      ';',
      $._expr,
      ']'
    ),

    struct_expr: $ => seq(
      $.IDENT,
      '{',
      repeat(seq(
        choice(
          $._expr,
          seq(
            $.IDENT,
            ':',
            $._ty,
          )),
        ',',
      )),
      '}',
    ),

    _atomic_expr_open: $ => choice(
      $.IDENT,
      $.group_expr,
      $.array_list_expr,
      $.array_replicate_expr,
      // $.struct_expr,
    ),

    call_expr: $ => seq($._suffix_expr_open, '(', ')'),

    _suffix_expr_open: $ => choice(
      $.call_expr,
      $._atomic_expr_open,
    ),

    // 左辺は closed でもいいけど、いまのところ multitive_expr は closed にならない。
    mul_expr_open: $ => seq($._multitive_expr, '*', $._suffix_expr_open),

    div_expr_open: $ => seq($._multitive_expr, '/', $._suffix_expr_open),

    _multitive_expr: $ => choice(
      $.mul_expr_open,
      $.div_expr_open,
      $._suffix_expr_open,
    ),

    add_expr_open: $ => seq($._additive_expr, '+', $._multitive_expr),

    sub_expr_open: $ => seq($._additive_expr, '-', $._multitive_expr),

    _additive_expr: $ => choice(
      $.add_expr_open,
      $.sub_expr_open,
      $._multitive_expr,
    ),

    arm_open: $ => seq($._pat, '=>', $._expr_open),

    arm_closed: $ => seq($._pat, '=>', $._expr_closed),

    _arms1: $ => choice(
      seq($.arm_open, optional(seq(',', $._arms1))),
      seq($.arm_closed, optional($._arms1)),
    ),

    match_expr: $ => seq('match', $._cond, '{', optional($._arms1), '}'),

    fn_expr_open: $ => seq('fn', '(', ')', $._expr_open),

    fn_expr_closed: $ => seq(
      'fn', '(', ')',
      choice(
        seq('->', $._ty, $.block),
        $._expr_closed,
      )),

    _expr_open: $ => choice(
      $._additive_expr,
      $.fn_expr_open,
    ),

    _expr_closed: $ => choice(
      $.block,
      $.match_expr,
      $.fn_expr_closed,
    ),

    _expr: $ => choice(
      $._expr_open,
      $._expr_closed,
    ),

    _cond: $ => $._expr,

    let_decl_open: $ => seq('let', $._pat, '=', $._expr_open),

    let_decl_closed: $ => seq('let', $._pat, '=', $._expr_closed),

    fn_decl: $ => seq('fn', $.IDENT, '(', ')', $.block),

    _decl_open: $ => choice(
      $.let_decl_open,
      $._expr_open,
    ),

    _decl_closed: $ => choice(
      $.let_decl_closed,
      $.fn_decl,
      $._expr_closed,
    ),

    _semi: $ => choice(
      seq($._decl_open, choice(seq(repeat1(';'), $._semi), repeat(';'))),
      seq($._decl_closed, repeat(';'), optional($._semi)),
    ),

    block: $ => seq('{', repeat(';'), optional($._semi), '}'),
  },
})
