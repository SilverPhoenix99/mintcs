class ModulePrinter
  Metadata = Struct.new(:name, :color) do
    def colorized(length = 0)
      return ' ' * length if name.length == 0
      length = [0, length - name.length - PADDING].max
      color = color_number(self.color) || color_number(:default)
      (' ' * PADDING) + "\e[3#{color}m#{name}\e[0m" + (' ' * length)
    end

    private def color_number(color)
      ModulePrinter::COLORS[ModulePrinter::COLOR_TYPES[color]]
    end
  end

  DEFAULT_METADATA = Metadata.new('', :default)

  PADDING = 2

  VISIBILITIES = %i(public protected private).freeze

  COLORS = {
    black:   0,
    red:     1,
    green:   2,
    yellow:  3,
    blue:    4,
    magenta: 5,
    cyan:    6,
    white:   7
  }.freeze

  COLOR_TYPES = {
    default:   :white,
    autoload:  :yellow,
    exception: :red,
    module:    :cyan,
    private:   :red,
    protected: :yellow,
    public:    :white
  }.freeze

  attr_reader :mod, :list

  def initialize(mod)
    @mod = mod
    initialize_mod_list
    nil
  end

  private def initialize_mod_list
    @list = {
      constants: get_mod_constants,
      methods: get_mod_methods
    }
    strip_module_methods
    nil
  end

  private def get_mod_constants
    @mod.constants(false).map(&:to_s)
  end

  private def get_mod_methods
    {
      class: VISIBILITIES.map { |type| [type, get_mod_methods_by_type(type)] }.to_h,
      instance: VISIBILITIES.map { |type| [type, get_mod_methods_by_type("#{type}_instance")] }.to_h
    }
  end

  private def get_mod_methods_by_type(type)
    @mod.send("#{type}_methods", false).map(&:to_s)
  end

  private def strip_module_methods
    return unless @mod.is_a?(Module)
    @list[:methods][:class].each do |_, list|
      list.reject! do |m|
        m = @mod.method(m).owner
        m == Module || m == Class
      end
    end
    nil
  end

  def self.list(mod)
    new(mod).list
  end

  def self.print(mod)
    printer = new(mod)
    printer.print_constants
    printer.print_class_methods
    printer.print_instance_methods
  end

  def self.print_constants(mod)
    new(mod).print_constants
  end

  def print_constants
    print_metadatas 'constants', constants_metadata
  end

  private def constants_metadata
    @list[:constants].map { |name| Metadata.new(name, constant_color(name)) }
  end

  private def constant_color(name)
    constant = (!@mod.autoload?(name) && (@mod.const_get(name) || true) rescue nil)

    return :autoload  unless constant
    return :exception if (constant < Exception rescue false)
    return :module    if (Module === @mod.const_get(name) rescue false)
    return :default
  end

  def self.print_class_methods(mod)
    new(mod).print_class_methods
  end

  def print_class_methods
    print_metadatas '::methods', methods_metadata(:class)
  end

  def self.print_instance_methods(mod)
    new(mod).print_instance_methods
  end

  def print_instance_methods
    print_metadatas '#methods', methods_metadata(:instance)
  end

  private def methods_metadata(type)
    @list[:methods][type].flat_map { |type, methods| methods.map { |name| Metadata.new(name, type) } }
  end

  private def print_metadatas(header, items)
    return if items.length == 0

    items = items.sort_by { |item| item.name.downcase }
    lengths = items.map { |items| items.name.length + PADDING }
    slices = calculate_slices(lengths)
    items = slice_items(items, slices)
    lengths = lengths.each_slice(slices).map(&:max)
    items = items.each.with_index.map { |items2, i| items2.map { |item| item.colorized(lengths[i]) } }
    lines = items.transpose.map(&:join)

    print_header header
    lines.each { |line| puts line }
    nil
  end

  private def calculate_slices(lengths)
    console_width = self.class.console_size[0]
    (1..lengths.count).each do |slice|
      width = lengths.each_slice(slice).map(&:max).reduce(&:+)
      return slice if width <= console_width
    end
    lengths.count
  end

  private def slice_items(items, slices)
    items = items.each_slice(slices).to_a
    if items.size > 1
      diff = items.first.size - items.last.size
      items.last.fill(DEFAULT_METADATA, items.last.length, diff) if diff > 0
    end
    items
  end

  private def print_header(header)
    return if header.length == 0

    color = COLORS[:blue]
    puts "\e[3#{color};1m#{header}\e[0m:"
  end

  def self.console_size
    `stty size`.split.map(&:to_i).reverse
  end
end

if $0 == __FILE__ && ARGV.size > 0

  ARGV.each.with_index do |module_name, i|
    puts

    mod = begin
      Object.const_get(module_name)
    rescue
      $stderr.puts "Invalid module: '#{module_name}'"
      next
    end

    if ARGV.size > 1
      separator = "=" * module_name.length
      puts module_name, separator
    end

    ModulePrinter.print mod
  end
end
