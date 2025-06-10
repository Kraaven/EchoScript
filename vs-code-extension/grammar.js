/**
 * @file A tree-sitter parser for Birch Programming language
 * @author kraaven
 * @license MIT
 */

/// <reference types="tree-sitter-cli/dsl" />
// @ts-check

module.exports = grammar({
  name: "birch",

  rules: {
    // TODO: add the actual grammar rules
    source_file: $ => "hello"
  }
});
