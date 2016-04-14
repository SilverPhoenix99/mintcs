class ListModule

  attr_reader :list

  def self.list(mod)
    new(mod).list
  end

  def self.write(mod, with_constants = false)
    puts new(mod).to_s(with_constants)
  end

  def initialize(mod)
    @mod = mod
    @list = @mod.instance_eval do
      {
        constants: constants(false).map(&:to_s).sort_by(&:downcase),
        methods: {
          class: {
            public: public_methods(false).map(&:to_s).sort,
            protected: protected_methods(false).map(&:to_s).sort,
            private: private_methods(false).map(&:to_s).sort
          },
          instance: {
            public: public_instance_methods(false).map(&:to_s).sort,
            protected: protected_instance_methods(false).map(&:to_s).sort,
            private: private_instance_methods(false).map(&:to_s).sort
          }
        }
      }
    end

    # strip Module/Class methods
    @list[:methods][:class].each { |_, list| list.reject! { |m| m = @mod.method(m).owner; m == Module || m == Class } } if @mod.is_a?(Module)

  end

  def to_s(with_constants = false)
    require 'pry'
    text = ""
    if with_constants
      list = format_constants
      text += Pry::Helpers.tablify_or_one_line(color(:bright_blue, 'constants'), list) unless list.empty?
    end
    list = format_class_methods
    text += Pry::Helpers.tablify_or_one_line(color(:bright_blue, '::methods'), list) unless list.empty?
    list = format_instance_methods
    text += Pry::Helpers.tablify_or_one_line(color(:bright_blue, '#methods'), list) unless list.empty?
    text
  end

  private
    def color(color, text)
      Pry::Helpers::Text.send(color, text)
    end

    def format_constants
      @list[:constants].map do |name|
        type = if const = (!@mod.autoload?(name) && (@mod.const_get(name) || true) rescue nil)
          case
            when (const < Exception rescue false)               then :red
            when (Module === @mod.const_get(name) rescue false) then :blue
            else :default
          end
        else
          :yellow
        end
        color(type, name)
      end
    end

    def format_class_methods
      format_methods(@list[:methods][:class])
    end

    def format_instance_methods
      format_methods(@list[:methods][:instance])
    end

    def format_methods(methods)
      methods.flat_map do |type, list|
        type = case type
          when :private   then :red
          when :protected then :yellow
          else :green
        end
        list.map { |method| color(type, method) }
      end
    end
end